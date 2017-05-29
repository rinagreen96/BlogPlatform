using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class Image
    {
        [Key]
        public int ImageId { get; set; }

        public int? ArticleId { get; set; }
        public Article Article { get; set; }

        public string image_path { get; set; }
    }
}
