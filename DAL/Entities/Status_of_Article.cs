using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class Status_of_Article
    {
        [Key]
        public int Status_of_ArticleId { get; set; }
        public string status_of_article_name { get; set; }

        public virtual ICollection<Article> Articles { get; set; }
    }
}
