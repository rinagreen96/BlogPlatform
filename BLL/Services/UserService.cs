using BLL.DTO;
using BLL.Infrastructure;
using BLL.Interfaces;
using DAL.Repositories;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace BLL.Services
{
    public class UserService: IUserService
    {
        IUnitOfWork Database { get; set; }

        public UserService(IUnitOfWork uow)
        {
            Database = uow;
        }

        private UserDTO GetUserDTOfromAppUser(ApplicationUser user)
        {
            UserDTO userDto = new UserDTO();
            ClientProfile client_profile = Database.ClientManager.Get(user.Email);
            userDto.Id = user.Id;
            userDto.avatar_path = client_profile.avatar_path;
            userDto.city = client_profile.city;
            userDto.date_of_registration = client_profile.date_of_registration;
            userDto.Email = client_profile.ApplicationUser.Email;
            userDto.password = client_profile.password;
            userDto.info = client_profile.info;
            userDto.name = client_profile.name;
            userDto.nickname = client_profile.nickname;
            userDto.sername = client_profile.sername;
            userDto.role = Database.RoleManager.FindById(client_profile.ApplicationUser.Roles.First().RoleId).Name;
            return userDto;
        }

        public async Task<OperationDetails> Create(UserDTO userDto)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userDto.Email);
            if (user == null)
            {
                user = new ApplicationUser { Email = userDto.Email, UserName = userDto.Email };
                var result = await Database.UserManager.CreateAsync(user, userDto.password);
                if (result.Errors.Count() > 0)
                    return new OperationDetails(false, result.Errors.FirstOrDefault(), "");
                // добавляем роль
                await Database.UserManager.AddToRoleAsync(user.Id, userDto.role);
                // создаем профиль клиента
                ClientProfile clientProfile = new ClientProfile { UserId = user.Id, avatar_path = userDto.avatar_path, city = userDto.city, date_of_registration=userDto.date_of_registration, info=userDto.info, name=userDto.name, sername = userDto.sername, nickname = userDto.nickname, password=userDto.password };
                Database.ClientManager.Create(clientProfile);
                Database.Save();
                return new OperationDetails(true, "Регистрация успешно пройдена", "");
            }
            else
            {
                return new OperationDetails(false, "Пользователь с таким логином уже существует", "Email");
            }
        }

        public async Task<OperationDetails> Update(UserDTO userDto)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(userDto.UserId);
            user.Email = userDto.Email;
            await Database.UserManager.UpdateAsync(user);
            ClientProfile clientProfile = Database.ClientManager.FindById(user.Id);
            clientProfile.avatar_path = userDto.avatar_path;
            clientProfile.city = userDto.city;
            clientProfile.info = userDto.info;
            clientProfile.name = userDto.name;
            clientProfile.nickname = userDto.nickname;
            clientProfile.sername = userDto.sername;
            clientProfile.password = userDto.password;
            Database.ClientManager.Update(clientProfile);
            
            Database.Save();
            return new OperationDetails(true, "Изменения внесены!", "");

        }

        public async Task<ClaimsIdentity> Authenticate(UserDTO userDto)
        {
            ClaimsIdentity claim = null;
            // находим пользователя
            ApplicationUser user = await Database.UserManager.FindAsync(userDto.Email, userDto.password);
            // авторизуем его и возвращаем объект ClaimsIdentity
            if (user != null)
                claim = await Database.UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
            return claim;
        }

        // начальная инициализация бд
        public async Task SetInitialData(UserDTO adminDto, List<string> roles)
        {
            foreach (string roleName in roles)
            {
                var role = await Database.RoleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    role = new ApplicationRole { Name = roleName };
                    await Database.RoleManager.CreateAsync(role);
                }
            }
            await Create(adminDto);
        }

        public UserDTO Find(string login)
        {
            UserDTO userDto = new UserDTO();
            ApplicationUser appuser = Database.UserManager.FindByEmail(login);
            return GetUserDTOfromAppUser(appuser);
        }

        public UserDTO FindById(string user_id)
        {
            UserDTO userDto = new UserDTO();
            ApplicationUser appuser = Database.UserManager.FindById(user_id);
            return GetUserDTOfromAppUser(appuser);
        }

        public List<UserDTO> GetAll()
        {
            List<UserDTO> users_DTO = new List<UserDTO>();
            var users = Database.UserManager.Users;
            foreach(var user in users)
            {
                users_DTO.Add(GetUserDTOfromAppUser(user));
            }
            return users_DTO;
        }

        public void ChangeRole(string user_id,string new_role)
        {
            string current_roleID = Database.UserManager.FindById(user_id).Roles.FirstOrDefault().RoleId;
            Database.UserManager.RemoveFromRole(user_id, Database.RoleManager.FindById(current_roleID).Name);
            Database.UserManager.AddToRole(user_id, new_role);
            Database.Save();
        }

        public void DeleteUser(string user_id)
        {
            Database.ClientManager.Delete(user_id);
            Database.Save();
            IBlogService blogService = new BlogService(Database);
            var blogs_for_user = Database.Blogs.GetAll().Where(x => x.ApplicationUserId == user_id);
            List<int> blogs_for_user_Ids = new List<int>();
            foreach (var blog in blogs_for_user)
            {
                blogs_for_user_Ids.Add(blog.BlogId);
            }
            foreach(int blog_id in blogs_for_user_Ids)
            {
                blogService.Delete(blog_id);
                Database.Save();
            }
            ApplicationUser user = Database.UserManager.FindById(user_id);
            Database.UserManager.Delete(user);
            Database.Save();
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
