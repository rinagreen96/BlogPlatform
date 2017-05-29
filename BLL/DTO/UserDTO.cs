using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace BLL.DTO
{
    public class UserDTO: IdentityUser
    {
        public string UserId { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string sername { get; set; }
        public string name { get; set; }
        public string nickname { get; set; }
        public DateTime date_of_registration { get; set; }
        public string city { get; set; }
        public string avatar_path { get; set; }
        public string info { get; set; }
    }
}
