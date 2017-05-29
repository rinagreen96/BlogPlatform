using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForBlogs.Models
{
    public class BlogInfoModel
    {
        public string UserId { get; set; }
        public string UserNick { get; set; }
        public List<int> BlogIds { get; set; }
        public ICollection<Tuple<string,string,DateTime?>> BlogCategoryDate;
    }
}