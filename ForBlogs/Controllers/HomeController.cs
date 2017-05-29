using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.Services;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ForBlogs.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            if (User.IsInRole("admin"))
            {
                return RedirectToAction("AdminPage", "Admin");
            }
            else
            {
                if (User.IsInRole("moderator"))
                {
                    return RedirectToAction("ModeratorPage", "Moderator");
                }
                else
                {
                    if (User.IsInRole("user"))
                    {
                        return RedirectToAction("ClientProfile", "Account", new { Email = User.Identity.Name, message = "" });
                    }

                    else
                    {
                        return View();
                    }
                }
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}