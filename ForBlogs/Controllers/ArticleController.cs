using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.Services;
using BLL.DTO;
using BLL.Interfaces;
using ForBlogs.Models;
using ForBlogs.Util;
using Ninject;
using System.IO;
using System.Web.Hosting;

namespace ForBlogs.Controllers
{
    public class ArticleController : Controller
    {
        // GET: Article
        IArticleService articleService;
        public ArticleController(IArticleService aservice)
        {
            articleService = aservice;
        }

        public ActionResult AddArticle(string blogname, int blog_id, string user_id)
        {
            ArticleModel articleModel = new ArticleModel();
            articleModel.BlogName = blogname;
            articleModel.BlogId = blog_id;
            articleModel.UserId = user_id;
            articleModel.ImagesPaths = new List<string>();
            articleModel.Status_of_ArticleId = 2;
            return View(articleModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddArticle(ArticleModel articleModel, HttpPostedFileBase[] upload)
        {
            if (ModelState.IsValid)
            {
                ArticleDTO articleDto = new ArticleDTO
                {
                    BlogId = articleModel.BlogId,
                    text = articleModel.text,
                    title = articleModel.title,
                    ImagePaths = new List<string>(),
                    Status_of_ArticleId = 2
                };
                articleService.Create(articleDto);
                string path = Server.MapPath("~/Galleries/"+ articleService.GetArticleIdWithTitleForBlog(articleModel.title, articleModel.BlogId).ArticleId);
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                
                if (this.Request.Files.AllKeys.Count()!=0 && upload[0]!=null)
                {
                    for (int i=0;i<this.Request.Files.Count;i++)
                    {
                        var file = this.Request.Files[i] as HttpPostedFileBase;
                        string fileName = file.FileName;
                        string path_img = "~/Galleries/" + articleService.GetArticleIdWithTitleForBlog(articleModel.title, articleModel.BlogId).ArticleId + "/" + fileName;
                        upload[i].SaveAs(Server.MapPath(path_img));
                        articleDto.ImagePaths.Add(path_img);
                    }
                }
                articleService.AddPictures(articleDto, articleService.GetArticleIdWithTitleForBlog(articleModel.title, articleModel.BlogId).ArticleId);
                articleService.AddTags(articleDto);
                articleService.ChangeBlogDateOfUpdate(articleDto.BlogId);
                return RedirectToAction("EditBlog", "Blog", new { blog_id = articleModel.BlogId, user_id =articleModel.UserId});
            }
            else { return View(articleModel); }
        }
        [HttpGet]
        public ActionResult GoToArticle(int article_id, string user_id)
        {
            ArticleModel article = new ArticleModel();
            ArticleDTO articleDTO = articleService.GetAll().Where(x => x.ArticleId == article_id).First();
            article.ArticleId = articleDTO.ArticleId;
            article.BlogId = articleDTO.BlogId;
            article.ImagesPaths = articleDTO.ImagePaths;
            article.title = articleDTO.title;
            article.text = articleDTO.text;
            article.Status_of_ArticleId = articleDTO.Status_of_ArticleId;
            article.comments = new List<Tuple<string,string, string, DateTime, int>>();
            foreach(var comment in articleDTO.Comments)
            {
                Tuple<string, string, string, DateTime, int> comment_art = new Tuple<string, string, string, DateTime, int>(comment.image_path, comment.user_name, comment.text, comment.datetime,comment.status_id);
                article.comments.Add(comment_art);
            }
            article.UserId = user_id;
            return View("EditArticle",article);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditArticle(ArticleModel articleModel, HttpPostedFileBase[] upload, string text)
        {
            if (ModelState.IsValid)
            {
                ArticleDTO articleDto = new ArticleDTO
                {
                    ArticleId = articleModel.ArticleId,
                    BlogId = articleModel.BlogId,
                    text = text,
                    title = articleModel.title,
                    ImagePaths = new List<string>(),
                    Status_of_ArticleId = 2
                };
                string path = Server.MapPath("~/Galleries/" + articleModel.ArticleId);
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                if (upload[0] != null)
                {
                    if (this.Request.Files.AllKeys.Count() != 0)
                    {
                        for (int i = 0; i < this.Request.Files.Count; i++)
                        {
                            var file = this.Request.Files[i] as HttpPostedFileBase;
                            string fileName = file.FileName;
                            string path_img = "~/Galleries/" + articleModel.ArticleId + "/" + fileName;
                            upload[i].SaveAs(Server.MapPath(path_img));
                            articleDto.ImagePaths.Add(path_img);
                        }
                    }
                    articleService.AddPictures(articleDto, articleModel.ArticleId);

                }
                articleService.AddTags(articleDto);
                articleService.Update(articleDto);
                articleService.ChangeBlogDateOfUpdate(articleDto.BlogId);
                return RedirectToAction("EditBlog", "Blog", new { blog_id = articleModel.BlogId });
            }
            else { return RedirectToAction("GoToArticle", "Article", new { article_id = articleModel.ArticleId, user_id = articleModel.UserId }); }
        }
        [HttpPost]
        public ActionResult DeleteArticle(int article_id, int blog_id)
        {
            ArticleDTO articleDTO = new ArticleDTO { ArticleId = article_id};
            articleService.Delete(article_id);
            articleService.ChangeBlogDateOfUpdate(blog_id);
            return RedirectToAction("DeleteArticle", "Blog", new { blog_id = blog_id});
        }
        [HttpPost]
        public string AddComment(string user_id, int article_id, string text, DateTime datetime, string user_name = "")
        {
            if(user_id=="")
            { user_id = articleService.GetUserIdByName(User.Identity.Name); }
            string image_path = "~/Images/"+user_id+".jpg";
            articleService.AddComment(user_id, article_id, image_path, text, datetime);
            return "Your comment was added, but not approved. Please, wait some time";
        }

        [HttpPost]
        public ActionResult SeeArticle(int article_id, string article_user_id)
        {
            ArticleModel article = new ArticleModel();
            ArticleDTO articleDTO = articleService.GetAll().Where(x => x.ArticleId == article_id).First();
            article.ArticleId = articleDTO.ArticleId;
            article.BlogId = articleDTO.BlogId;
            article.ImagesPaths = articleDTO.ImagePaths;
            article.title = articleDTO.title;
            article.text = articleDTO.text;
            article.Status_of_ArticleId = articleDTO.Status_of_ArticleId;
            article.comments = new List<Tuple<string, string, string, DateTime, int>>();
            foreach (var comment in articleDTO.Comments)
            {
                Tuple<string, string, string, DateTime, int> comment_art = new Tuple<string, string, string, DateTime, int>(comment.image_path, comment.user_name, comment.text, comment.datetime, comment.status_id);
                article.comments.Add(comment_art);
            }
            article.UserId = article_user_id;
            return View("SeeArticle", article);
        }

        [HttpPost]
        public void DeletePicture(string imagepath, int article_id)
        {
            articleService.DeletePicture(imagepath, article_id);
            articleService.ChangeBlogDateOfUpdate(articleService.GetAll().Where(x=>x.ArticleId==article_id).First().BlogId);
        }
    }
}