using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class Status_of_Comment
    {
        [Key]
        public int Status_of_CommentId { get; set; }
        public string status_of_comment_name { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
