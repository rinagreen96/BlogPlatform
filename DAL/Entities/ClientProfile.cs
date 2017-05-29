using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class ClientProfile
    {
        [Key]
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        public string sername { get; set; }
        public string name { get; set; }
        //public string login { get; set; }
        public string password { get; set; }
        public string nickname { get; set; }
        //public string email { get; set; }
        public DateTime date_of_registration { get; set; }
        public string city { get; set; }
        public string avatar_path { get; set; }
        public string info { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
