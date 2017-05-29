using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string category_name { get; set; }
        public ICollection<int> BlogIds;
    }
}
