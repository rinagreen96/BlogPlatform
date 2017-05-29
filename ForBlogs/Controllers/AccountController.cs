using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using ForBlogs.Models;
using BLL.DTO;
using System.Security.Claims;
using BLL.Interfaces;
using BLL.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System;

namespace UserStore.Controllers
{
    public class AccountController : Controller
    {
        private IUserService UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //---------------------------------------
        private ClientProfileModel ReturnClientProfile(UserDTO userDto)
        {
            ClientProfileModel clientProfile = new ClientProfileModel();
            clientProfile.ID = userDto.Id;
            clientProfile.Avatar_path = userDto.avatar_path;
            clientProfile.City = userDto.city;
            clientProfile.Date_of_registration = userDto.date_of_registration;
            clientProfile.Email = userDto.Email;
            clientProfile.Password = userDto.password;
            clientProfile.Info = userDto.info;
            clientProfile.Name = userDto.name;
            clientProfile.Nickname = userDto.nickname;
            clientProfile.Sername = userDto.sername;
            return clientProfile;
        }
        //---------------------------------------

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult GoToBlogs(string UserEmail)
        {
            return RedirectToAction("Index", "Blog", new { UserId=UserService.Find(UserEmail).Id, UserNick = UserService.Find(UserEmail).nickname });
        }
        [HttpPost]
        public string CheckNickname(string nick)
        {
            if(UserService.GetAll().Find(x=>x.nickname==nick)!=null)
            {
                return "User with such nickname already exists!";
            }
            else
            {
                return "";
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            await SetInitialDataAsync();
            if (ModelState.IsValid)
            {
                UserDTO userDto = new UserDTO { Email = model.Email, password = model.Password };
                if (userDto != null)
                {
                    ClaimsIdentity claim = await UserService.Authenticate(userDto);
                    if (claim == null)
                    {
                        ModelState.AddModelError("", "Неверный логин или пароль.");
                    }
                    
                    else
                    {
                        AuthenticationManager.SignOut();
                        AuthenticationManager.SignIn(new AuthenticationProperties
                        {
                            IsPersistent = true
                        }, claim);
                        string user_role = UserService.Find(userDto.Email).role;
                        if (user_role=="admin")
                        {
                            return RedirectToAction("AdminPage", "Admin");
                        }
                        else
                        {
                            if (user_role == "moderator")
                            {
                                return RedirectToAction("ModeratorPage", "Moderator");
                            }
                            else
                            {
                                string message = "Welcome to Your Profile, " + userDto.nickname;
                                return RedirectToAction("ClientProfile", new { Email = userDto.Email, message = message });
                            }
                        }

                    }
                }
                else { return RedirectToAction("Login"); }
            }
            return View(model);
        }

        public ActionResult ClientProfile(string Email)
        {
            UserDTO userDto = new UserDTO();
            try
            {
                userDto = UserService.Find(Email);
            }
            catch (NullReferenceException ex)
            { return RedirectToAction("Login"); }
            ClientProfileModel clientProfile = ReturnClientProfile(userDto);
            ViewBag.CurrentUserName = User.Identity.Name;
            return View("ViewClientProfile",clientProfile);
        }
        [HttpGet]
        public ActionResult EditClientProfile(string Email)
        {
            UserDTO userDto = new UserDTO();
            try
            {
                userDto = UserService.Find(Email);
            }
            catch (NullReferenceException ex)
            { return RedirectToAction("Login"); }
            ClientProfileModel clientProfile = ReturnClientProfile(userDto);
            return View("ClientProfile", clientProfile);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditClientProfile(ClientProfileModel model, HttpPostedFileBase upload, string info)
        {
            string new_avatar_path = model.Avatar_path;
            string fileName = "";
            if (upload != null)
            {
                fileName = System.IO.Path.GetFileName(upload.FileName);
                // сохраняем файл в папку Images в проекте
                upload.SaveAs(Server.MapPath("~/Images/" + model.ID+ ".jpg"));
            }
            if(fileName!="")
            {
                new_avatar_path = "~/Images/" + fileName;
            }
            await SetInitialDataAsync();
            if (ModelState.IsValid)
            {
                UserDTO userDto = new UserDTO
                {
                    UserId = model.ID,
                    Email = model.Email,
                    password = model.Password,
                    city = model.City,
                    name = model.Name,
                    sername = model.Sername,
                    role = "user",
                    nickname = model.Nickname,
                    date_of_registration = model.Date_of_registration,
                    avatar_path = new_avatar_path,
                    info = info
                };

                OperationDetails operationDetails = await UserService.Update(userDto);
                if (operationDetails.Succedeed)
                    return RedirectToAction("ClientProfile", new { Email = userDto.Email });
                else
                    ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Register(RegisterModel model, HttpPostedFileBase upload, string info)
        {
            
            await SetInitialDataAsync();
            if (ModelState.IsValid)
            {
                UserDTO userDto = new UserDTO();
                userDto.avatar_path = "~/Images/ForSite/default.png";
                if (upload != null)
                {
                    // сохраняем файл в папку Images в проекте
                    upload.SaveAs(Server.MapPath("~/Images/" + userDto.UserId + ".jpg"));
                    userDto.avatar_path = "~/Images/" + userDto.UserId + ".jpg";
                }
                userDto.UserId = userDto.UserId;
                userDto.Email = model.Email;
                userDto.password = model.Password;
                userDto.city = model.City;
                userDto.name = model.Name;
                userDto.sername = model.Sername;
                userDto.role = "user";
                userDto.nickname = model.Nickname;
                userDto.date_of_registration = System.DateTime.Now;
                userDto.info = info;
                
                OperationDetails operationDetails = await UserService.Create(userDto);
                if (operationDetails.Succedeed)
                    return View("Login");
                else
                    ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
            }
            return View(model);
        }
        private async Task SetInitialDataAsync()
        {
            await UserService.SetInitialData(new UserDTO
            {
                Email = "somemail@mail.ru",
                UserName = "somemail@mail.ru",
                password = "ad46D_ewr3",
                name = "Семен",
                sername = "Горбунков",
                city = "Киев",
                role = "admin",
                date_of_registration = System.DateTime.Now,
                info = "bla-bla-bla"
            }, new List<string> { "admin" });

            await UserService.SetInitialData(new UserDTO
            {
                Email = "moderator@mail.ru",
                UserName = "moderator@mail.ru",
                password = "12345678",
                name = "Вася",
                sername = "Пупкин",
                city = "Киев",
                role = "moderator",
                
                date_of_registration = System.DateTime.Now,
                info = "bla-bla-bla_2"
            }, new List<string> { "moderator" });

            await UserService.SetInitialData(new UserDTO
            {
                Email = "somebody1@mail.ru",
                UserName = "somebody1@mail.ru",
                password = "123456",
                name = "Nikky",
                sername = "Sherry",
                city = "Kiev",
                role = "user",
                avatar_path = "~/Images/_q98XLYKBI4.jpg",
                nickname = "Sher",
                date_of_registration = System.DateTime.Now,
                info = "bla-bla-bla_3"
            }, new List<string> { "user" });
        }
         
        public ActionResult AdminPage()
        {
            return View();
        }

        public ActionResult SeeAllBlogs()
        {
            return RedirectToAction("SeeAllBlogs", "Blog");
        }
    }
}