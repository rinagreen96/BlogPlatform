using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Infrastructure;
using BLL.DTO;
using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IArticleService
    {
        void Create(ArticleDTO articleDto);
        void Update(ArticleDTO articleDto);
        void Delete(int article_id);
        void AddPictures(ArticleDTO articleDto, int article_id);
        void DeletePicture(string imagepath, int article_id);
        void AddTags(ArticleDTO articleDto);
        ArticleDTO GetArticleIdWithTitleForBlog(string title, int blog_id);
        ICollection<ArticleDTO> GetAll();
        string GetUserIdByName(string name);
        void ChangeStatus(int article_id, int status_id);
        void ChangeStatusOfComment(int comment_id, int status_id);
        void AddComment(string user_id, int article_id, string image_path, string text, DateTime datetime);
        ICollection<CommentDTO> GetCommentsForArticle(int article_id);
        void ChangeBlogDateOfUpdate(int blog_id);
    }
}
