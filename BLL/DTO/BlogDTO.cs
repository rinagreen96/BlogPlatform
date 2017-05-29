using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class BlogDTO
    {
        public int Id { get; set; }
        public string BlogName { get; set; }
        public DateTime? date_of_creation { get; set; }
        public DateTime? date_of_last_update { get; set; }
        public string UserId { get; set; }
        public int CategoryId { get; set; }
        public string blogDescription { get; set; }
        public ICollection<int> ArticleIds;
    }
}
