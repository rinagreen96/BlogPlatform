using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForBlogs.Models
{
    public class CommentModel
    {
        public int comment_id { get; set; }
        public string imagepath { get; set; }
        public string user_name { get; set; }
        [Required]
        public string text { get; set; }
        public int status_id { get; set; }
        public DateTime datetime { get; set; }
    }
}