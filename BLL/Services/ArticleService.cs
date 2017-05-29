using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Entities;
using BLL.Interfaces;
using BLL.Infrastructure;
using BLL.DTO;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.Hosting;

namespace BLL.Services
{
    public class ArticleService : IArticleService
    {
        IUnitOfWork Database { get; set; }

        public ArticleService(IUnitOfWork uow)
        {
            Database = uow;
        }
        public void Create(ArticleDTO articleDto)
        {
            Article article = new Article();
                Article last_article = Database.Articles.GetAll().LastOrDefault();
                if (last_article != null)
                {
                    article.Id = last_article.Id + 1;
                }
                else
                {
                    article.Id = 1;
                }
                article.Blog = Database.Blogs.Find(x => x.BlogId == articleDto.BlogId).FirstOrDefault();
                article.Status_of_Article = Database.Status_of_Articles.Find(x => x.Status_of_ArticleId == articleDto.Status_of_ArticleId).FirstOrDefault();
                article.text = articleDto.text;
                article.title = articleDto.title;
                article.Comments = new List<Comment>();
                article.Images = new List<Image>();
                Database.Articles.Create(article);
                Database.Save();
            
        }

        public void Update(ArticleDTO articleDto)
        {
            Article article = Database.Articles.GetAll().Where(x => x.Id == articleDto.ArticleId).FirstOrDefault();

            article.Status_of_Article = Database.Status_of_Articles.Find(x => x.Status_of_ArticleId == articleDto.Status_of_ArticleId).FirstOrDefault();
            article.text = articleDto.text;
            article.title = articleDto.title;
            article.Comments = new List<Comment>();
            Database.Articles.Update(article);
            Database.Save();

        }

        public void Delete(int article_id)
        {
            Article article = Database.Articles.GetAll().Where(x => x.Id == article_id).FirstOrDefault();
            List<int> imageids = new List<int>();
            foreach(var image in article.Images.ToList())
            {
                imageids.Add(image.ImageId);
            }
             foreach (var id in imageids)
            {
                
                DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.ApplicationPhysicalPath + @"Galleries\" + article_id);
                dirInfo.GetFiles().Where(x => x.Name == Database.Images.Get(id).image_path.Split('/').Last()).First().Delete();
                if (dirInfo.GetFiles().Count() == 0)
                {
                    dirInfo.Delete();
                }
                Database.Images.Delete(id);
            }
            Database.Articles.Delete(article.Id);
            Database.Save();
        }

        public void AddPictures(ArticleDTO articleDto, int article_id)
        {

            Article article = Database.Articles.Get(article_id);
            if (articleDto.ImagePaths == null)
            {
                articleDto.ImagePaths = new List<string>();
            }
            if (articleDto.ImagePaths.Count() != 0)
            {
                foreach (var image in articleDto.ImagePaths)
                {
                    Image last_image = Database.Images.GetAll().LastOrDefault();
                    int image_id = 1;
                    if (last_image != null)
                    {
                        image_id = last_image.ImageId;
                    }
                    Image img = new Image { image_path = image, ImageId = image_id, Article = article };
                    Database.Images.Create(img);
                    article.Images.Add(img);
                    article.Status_of_Article = Database.Status_of_Articles.Get(2);
                    Database.Articles.Update(article);
                    Database.Save();

                }

            }
        }

        public void DeletePicture(string imagepath, int article_id)
        {
            Image img = Database.Images.GetAll().Where(x => x.image_path == imagepath).FirstOrDefault();
            Article article = Database.Articles.Get(article_id);
            article.Images.Remove(img);
            Database.Articles.Update(article);
            Database.Save();
            Database.Images.Delete(img.ImageId);
            Database.Save();
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.ApplicationPhysicalPath + @"Galleries\" + article_id);
            dirInfo.GetFiles().Where(x => x.Name == imagepath.Split('/').Last()).First().Delete();
            if (dirInfo.GetFiles().Count() == 0)
            {
                dirInfo.Delete();
            }
        }

        public ArticleDTO GetArticleIdWithTitleForBlog(string title, int blog_id)
        {
            Article article = Database.Articles.GetAll().Where(x => (x.Blog.BlogId == blog_id && x.title == title)).FirstOrDefault();
            ArticleDTO articleDTO = new ArticleDTO
            {
                ArticleId = article.Id,
                BlogId = article.Blog.BlogId,
                Comments = new List<CommentDTO>(),
            ImagePaths = new List<string>(),
                text = article.text,
                title = article.title
            };
            foreach (var image in article.Images)
            {
                articleDTO.ImagePaths.Add(image.image_path);
            }
            foreach (var comment in article.Comments)
            {
                CommentDTO commentDTO = new CommentDTO();
                commentDTO.article_id = comment.Article.Id;
                commentDTO.comment_id = comment.CommentId;
                commentDTO.datetime = comment.date_of_comment;
                commentDTO.image_path = comment.ApplicationUser.ClientProfile.avatar_path;
                commentDTO.status_id = comment.Status_of_Comment.Status_of_CommentId;
                commentDTO.text = comment.text;
                commentDTO.user_name = comment.ApplicationUser.Email;
                articleDTO.Comments.Add(commentDTO);
            }
            return articleDTO;
        }

        public ICollection<ArticleDTO> GetAll()
        {
            ICollection<Article> articles = Database.Articles.GetAll().ToList();
            ICollection<ArticleDTO> articlesDTO = new List<ArticleDTO>();
            foreach (var article in articles)
            {
                ArticleDTO articleDTO = new ArticleDTO();
                articleDTO.ArticleId = article.Id;
                articleDTO.BlogId = article.Blog.BlogId;
                articleDTO.Comments = new List<CommentDTO>();
                foreach (var comment in article.Comments)
                {
                    CommentDTO commentDTO = new CommentDTO();
                    commentDTO.article_id = comment.Article.Id;
                    commentDTO.comment_id = comment.CommentId;
                    commentDTO.datetime = comment.date_of_comment;
                    commentDTO.image_path = comment.ApplicationUser.ClientProfile.avatar_path;
                    commentDTO.status_id = comment.Status_of_Comment.Status_of_CommentId;
                    commentDTO.text = comment.text;
                    commentDTO.user_name = comment.ApplicationUser.Email;
                    articleDTO.Comments.Add(commentDTO);
                }
                articleDTO.ImagePaths = new List<string>();
                foreach (var image in article.Images)
                {
                    articleDTO.ImagePaths.Add(image.image_path);
                }
                articleDTO.Status_of_ArticleId = article.Status_of_Article.Status_of_ArticleId;
                articleDTO.text = article.text;
                articleDTO.title = article.title;
                articlesDTO.Add(articleDTO);
            }
            return (articlesDTO);
        }

        public void ChangeStatus(int article_id, int status_id)
        {
            Article article = Database.Articles.GetAll().Where(x => x.Id == article_id).FirstOrDefault();

            article.Status_of_Article = Database.Status_of_Articles.Find(x => x.Status_of_ArticleId == status_id).First();
            Database.Articles.Update(article);
            Database.Save();
        }

        public void AddComment(string user_id, int article_id, string image_path, string text, DateTime datetime)
        {
            Comment new_comment = new Comment { Article = Database.Articles.Get(article_id), date_of_comment = datetime, text = text, Status_of_Comment = Database.Status_of_Comments.Get(2), ApplicationUser = Database.UserManager.Users.Where(x=>x.Id==user_id).First()};
            Database.Comments.Create(new_comment);
            Database.Save();
        }

        public ICollection<CommentDTO> GetCommentsForArticle(int article_id)
        {
            var comments_for_article = Database.Comments.GetAll().Where(x=>x.Article.Id==article_id);
            ICollection<CommentDTO> commentsDTO_forAtricle = new List<CommentDTO>();
            foreach (var comment in comments_for_article)
            {
                CommentDTO commentDTO = new CommentDTO { article_id = article_id,
                    comment_id = comment.CommentId,
                    datetime = comment.date_of_comment,
                    status_id = comment.Status_of_Comment.Status_of_CommentId,
                    text = comment.text,
                user_name = comment.ApplicationUser.Email};
                commentsDTO_forAtricle.Add(commentDTO);
            }
            return commentsDTO_forAtricle;
        }

        public void ChangeStatusOfComment(int comment_id, int status_id)
        {
            Comment comment = Database.Comments.GetAll().Where(x => x.CommentId == comment_id).FirstOrDefault();

            comment.Status_of_Comment = Database.Status_of_Comments.Find(x => x.Status_of_CommentId == status_id).First();
            Database.Comments.Update(comment);
            Database.Save();
        }

        public void ChangeBlogDateOfUpdate(int blog_id)
        {
            Blog blog = Database.Blogs.Get(blog_id);
            blog.date_of_last_update = DateTime.Now;
            Database.Blogs.Update(blog);
            Database.Save();
        }

        public string GetUserIdByName(string name)
        {
            return Database.UserManager.Users.Where(x => x.Email == name).First().Id;
        }

        public void AddTags(ArticleDTO articleDto)
        {
            List<string> tags = new List<string>();
            Regex regex_for_tags = new Regex(@"\B#\w{0,}");
            if (articleDto.text != "")
            {
                MatchCollection matches = regex_for_tags.Matches(articleDto.text);
                if (matches.Count != 0)
                {
                    foreach (var match in matches)
                    {
                        if (Database.Tags.GetAll().Where(x => x.name == match.ToString()).Count()==0)
                        {
                            Tag tag = new Tag();
                            tag.name = match.ToString();
                            tag.Articles.Add(Database.Articles.Get(articleDto.ArticleId));
                            Database.Tags.Create(tag);
                            Database.Save();
                        }
                        else
                        {
                            Tag tag = Database.Tags.GetAll().Where(x => x.name == match.ToString()).First();
                            tag.Articles.Add(Database.Articles.Get(articleDto.ArticleId));
                            Database.Tags.Update(tag);
                            Database.Save();
                        }
                    }
                }
            }
        }
    }


}
