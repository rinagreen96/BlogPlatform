using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class ArticleDTO
    {
        public int ArticleId { get; set; }
        public string title { get; set; }
        public string text { get; set; }

        public int BlogId { get; set; }

        public int Status_of_ArticleId { get; set; } 

        public virtual ICollection<CommentDTO> Comments { get; set; }

        public virtual List<string> ImagePaths { get; set; }
    }
}
