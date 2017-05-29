using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Infrastructure;
using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IBlogService: IDisposable
    {
        void Create(BlogDTO blogDto);
        void Delete(int blog_id);
        void EditDescription(BlogDTO blog);
        CategoryDTO GetCategoryForBlog(int BlogId);
        string GetUserNicknameForBlog(int BlogId);
        string GetUserIdForBlog(int BlogId);
        UserDTO GetUserById(string id);
        UserDTO GetUserByName(string name);
        IEnumerable<BlogDTO> GetAllForUser(string userId);
        IEnumerable<BlogDTO> GetAll();
        ICollection<CategoryDTO> GetAllCategories();
        ICollection<ArticleDTO> GetArticlesForBlog(int BlogId);
        ICollection<ArticleDTO> GetArticlesWithTags(string tags, List<int?> BlogIds_notconsidered=null, int BlogId=0);
        List<string> Recommended_materials(List<string> list_of_docs, string UserName);
    }
}
