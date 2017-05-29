using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForBlogs.Models
{
    public class BlogEditModel
    {
        [Required(ErrorMessage ="BlogName is a required field!")]
        public string BlogName { get; set; }
        public int BlogId { get; set; }
        public string BlogDescription { get; set; }
        public string CategoryName { get; set; }
        public ICollection<ArticleModel> articles { get; set; }
    }
}