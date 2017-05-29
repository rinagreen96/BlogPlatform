using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.Interfaces;
using ForBlogs.Models;

namespace ForBlogs.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        IUserService userService;
        public AdminController(IUserService uservice)
        {
            userService = uservice;
        }
        public ActionResult AdminPage()
        {
            List<UserModel> all_users_notadmin = new List<UserModel>();
            var all_users_notadmin_DTO = userService.GetAll().Where(x => x.role != "admin");
            foreach (var user_notadmin_DTO in all_users_notadmin_DTO)
            {
                UserModel user_notadmin = new UserModel();
                user_notadmin.Email = user_notadmin_DTO.Email;
                user_notadmin.RoleName = user_notadmin_DTO.role;
                user_notadmin.UserID = user_notadmin_DTO.Id;
                all_users_notadmin.Add(user_notadmin);
            }
            return View(all_users_notadmin);
        }

        public void ChangeRole(string user_id, string new_role)
        {
            userService.ChangeRole(user_id, new_role);
            //return ("Changes are added!");
        }
        [HttpPost]
        public ActionResult DeleteUser(string user_id)
        {
            string email_deletedPerson = userService.FindById(user_id).email;
            userService.DeleteUser(user_id);
            return RedirectToAction("AdminPage");
        }
    }
}