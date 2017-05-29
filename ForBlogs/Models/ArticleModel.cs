using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForBlogs.Models
{
    public class ArticleModel
    {
        public int ArticleId { get; set; }

        [Required(ErrorMessage = "Title is a requiered field!")]
        public string title { get; set; }
        public string text { get; set; }

        public int BlogId { get; set; }
        public string BlogName { get; set; }
        public string UserId { get; set; }
        public List<Tuple<string,string,string,DateTime,int>> comments { get; set; }
        public List<string> ImagesPaths;
        public int Status_of_ArticleId { get; set; }
    }
}