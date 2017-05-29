using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForBlogs.Models
{
    public class BlogModel
    {
        public int Id { get; set; }
        public string Blogname { get; set; }
        public string BlogDescription { get; set; }
        public DateTime? date_of_creation { get; set; }
        public DateTime? date_of_last_update { get; set; }
        public string UserNickname { get; set; }
        public string CategoryName { get; set; }
        public string UserId { get; set; }        
    }
}