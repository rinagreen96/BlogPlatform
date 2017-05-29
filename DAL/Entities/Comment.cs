using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string text { get; set; }
        public DateTime date_of_comment { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int? Status_of_CommentId { get; set; }
        public virtual Status_of_Comment Status_of_Comment { get; set; }

        public int? ArticleId { get; set; }
        public virtual Article Article { get; set; }
    }
}
