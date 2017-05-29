using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string category_name { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; }
    }
}
