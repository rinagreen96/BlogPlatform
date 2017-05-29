using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.Interfaces;
using BLL.DTO;
using ForBlogs.Models;

namespace ForBlogs.Controllers
{
    public class ModeratorController : Controller
    {
        // GET: Moderator
        IArticleService articleService;
        IModeratorService moderatorService;
        public ModeratorController(IArticleService aservice, IModeratorService mService)
        {
            articleService = aservice;
            moderatorService = mService;
        }
        public ActionResult ModeratorPage()
        {
            List<ArticleModel> all_articles_notapproved = new List<ArticleModel>();
            var all_articles_notapproved_DTO = articleService.GetAll().Where(x => x.Status_of_ArticleId == 2);
            foreach(var article_notapproved_DTO in all_articles_notapproved_DTO)
            {
                ArticleModel article_notapproved = new ArticleModel();
                article_notapproved.ArticleId = article_notapproved_DTO.ArticleId;
                article_notapproved.BlogId = article_notapproved_DTO.BlogId;
                article_notapproved.text = article_notapproved_DTO.text;
                article_notapproved.title = article_notapproved_DTO.title;
                article_notapproved.ImagesPaths = article_notapproved_DTO.ImagePaths;
                article_notapproved.Status_of_ArticleId = article_notapproved_DTO.Status_of_ArticleId;
                all_articles_notapproved.Add(article_notapproved);
            }
            
            List<CommentModel> all_comments_notapproved = new List<CommentModel>();
            var all_articles = articleService.GetAll();
            foreach(var article in all_articles)
            {
                var comments_forcurrentArticle = articleService.GetCommentsForArticle(article.ArticleId);
                foreach(var comment_for_currentArticle in comments_forcurrentArticle)
                {
                    CommentModel comment = new CommentModel
                    {
                        comment_id = comment_for_currentArticle.comment_id,
                        imagepath = comment_for_currentArticle.image_path,
                        datetime = comment_for_currentArticle.datetime,
                        status_id = comment_for_currentArticle.status_id,
                        text = comment_for_currentArticle.text,
                        user_name = comment_for_currentArticle.user_name
                    };
                    if(comment.status_id==2)
                    {
                        all_comments_notapproved.Add(comment);
                    }
                }
            }
            ViewBag.Comments = all_comments_notapproved;
            return View(all_articles_notapproved);
        }

        public ActionResult Approve(int article_id)
        {
            articleService.ChangeStatus(article_id, 1);
            return RedirectToAction("ModeratorPage");
        }

        public ActionResult Reject(int article_id)
        {
            articleService.ChangeStatus(article_id, 3);
            moderatorService.SendingEmailArticle("rejected", article_id);
            return RedirectToAction("ModeratorPage");
        }

        public ActionResult ApproveComment(int comment_id)
        {
            articleService.ChangeStatusOfComment(comment_id, 1);
            return RedirectToAction("ModeratorPage");
        }

        public ActionResult RejectComment(int comment_id)
        {
            articleService.ChangeStatusOfComment(comment_id, 3);
            moderatorService.SendingEmailComment("rejected", comment_id);
            return RedirectToAction("ModeratorPage");
        }
    }
}