using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class CommentDTO
    {
        public int comment_id { get; set; }
        public int article_id { get; set; }
        public string user_name { get; set; }
        public string image_path { get; set; }
        public string text { get; set; }
        public DateTime datetime { get; set; }
        public int status_id { get; set; }
    }
}
