using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class Blog
    {
        [Key]
        public int BlogId { get; set; }
        public string BlogName { get; set; }
        public string BlogDescription { get; set; }
        public DateTime? date_of_creation { get; set; }
        public DateTime? date_of_last_update { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int? CategoryID { get; set; }
        public virtual Category Category { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
        public Blog()
        {
            Articles = new List<Article>();
        }
    }
}
