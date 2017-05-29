using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        public string title { get; set; }
        public string text { get; set; }

        public int? BlogId { get; set; }
        public virtual Blog Blog { get; set; }

        public int? Status_of_ArticleId { get; set; }
        public virtual Status_of_Article Status_of_Article { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Image> Images { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
        public Article()
        {
            Tags = new List<Tag>();
            Images = new List<Image>();
            Comments = new List<Comment>();
        }
    }
}
