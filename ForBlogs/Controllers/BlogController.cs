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

namespace ForBlogs.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog
        IBlogService blogService;
        public BlogController(IBlogService bservice)
        {
            blogService = bservice;
        }
        public ActionResult Index(string UserId, string UserNick)
        {
            IEnumerable<BlogDTO> blogDTOs = blogService.GetAllForUser(UserId);
            BlogInfoModel blogInfoModel = new BlogInfoModel();
            blogInfoModel.UserId = UserId;
            blogInfoModel.UserNick = UserNick;
            blogInfoModel.BlogIds = new List<int>();
            blogInfoModel.BlogCategoryDate = new List<Tuple<string,string,DateTime?>>();
            if (blogDTOs.Count()!=0)
            {
                foreach (var blogDTO in blogDTOs)
                {
                    blogInfoModel.BlogIds.Add(blogDTO.Id);
                    blogInfoModel.BlogCategoryDate.Add(new Tuple<string,string,DateTime?>(blogDTO.BlogName,blogService.GetCategoryForBlog(blogDTO.Id).category_name, blogDTO.date_of_last_update));
                }
            }
            if (blogDTOs.Count() != 0)
            {
                List<string> all_articles = new List<string>();
                foreach (var blog in blogDTOs)
                {
                    var articles_for_currentblog = blogService.GetArticlesForBlog(blog.Id);
                    foreach (var article in articles_for_currentblog)
                    {
                        all_articles.Add(article.text);
                    }
                }
                List<ArticleModel> articlesmodels_with_tags = new List<ArticleModel>();
                string tags = "";
                if (all_articles.Count() > 1)
                {
                    var tags_array = blogService.Recommended_materials(all_articles, User.Identity.Name);
                    foreach (var tag in tags_array)
                    {
                        tags += tag + " ";
                    }
                    var blogs_notConsidered = blogService.GetAllForUser(blogService.GetUserByName(User.Identity.Name).Id).ToList();
                    List <int?> blogs_notConsidered_Ids = new List<int?>();
                    foreach(var b in blogs_notConsidered)
                    {
                        blogs_notConsidered_Ids.Add(b.Id);
                    }
                    List <ArticleDTO> articles_with_tags = blogService.GetArticlesWithTags(tags, blogs_notConsidered_Ids,0).ToList();
                    
                    foreach (var article in articles_with_tags)
                    {
                        if (article.Status_of_ArticleId == 1)
                        {
                            ArticleModel art_model = new ArticleModel();
                            art_model.ArticleId = article.ArticleId;
                            art_model.BlogId = article.BlogId;
                            art_model.BlogName = blogService.GetAll().Where(x => x.Id == article.BlogId).First().BlogName;
                            foreach (var comment in article.Comments)
                            {
                                if (comment.status_id == 1)
                                {
                                    Tuple<string, string, string, DateTime, int> comment_art = new Tuple<string, string, string, DateTime, int>(comment.image_path, comment.user_name, comment.text, comment.datetime, comment.status_id);
                                    art_model.comments.Add(comment_art);
                                }
                            }
                            art_model.ImagesPaths = article.ImagePaths;
                            art_model.text = article.text;
                            art_model.title = article.title;
                            art_model.UserId = blogService.GetUserIdForBlog(article.BlogId);
                            art_model.Status_of_ArticleId = article.Status_of_ArticleId;
                            articlesmodels_with_tags.Add(art_model);
                        }
                    }
                }
                ViewBag.Interesting = articlesmodels_with_tags;
            }
            return View(blogInfoModel);
        }
        public ActionResult Create(string user_id, string user_nick)
        {
            BlogModel new_BM = new BlogModel { UserId = user_id, UserNickname = user_nick, BlogDescription="", date_of_creation = DateTime.Now, date_of_last_update = DateTime.Now};
            ICollection<CategoryDTO> categories = blogService.GetAllCategories();
            ViewBag.Categories = categories;
            return View(new_BM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(BlogModel blogModel)
        {
            if (ModelState.IsValid)
            {
                BlogDTO blogDto = new BlogDTO
                {
                    Id = blogModel.Id,
                    BlogName = blogModel.Blogname,
                    blogDescription=blogModel.BlogDescription,
                    date_of_creation = blogModel.date_of_creation,
                    date_of_last_update = blogModel.date_of_last_update,
                    UserId = blogModel.UserId,
                    CategoryId = blogService.GetAllCategories().Where(x => x.category_name == blogModel.CategoryName).FirstOrDefault().CategoryId
                };
                blogService.Create(blogDto);
            }
            return RedirectToAction("Index", "Blog", new { UserId = blogModel.UserId, UserNick = blogModel.UserNickname });
        }
        //[HttpPost]
        public ActionResult EditBlog(int blog_id)
        {
            BlogEditModel new_BEM = new BlogEditModel();
            string user_id = blogService.GetUserIdForBlog(blog_id);
            new_BEM.BlogName = blogService.GetAllForUser(user_id).Where(x => x.Id == blog_id).First().BlogName;
            new_BEM.BlogId = blog_id;
            new_BEM.CategoryName = blogService.GetCategoryForBlog(blog_id).category_name;
            new_BEM.BlogDescription = blogService.GetAllForUser(user_id).Where(x => x.Id == blog_id).First().blogDescription;
            new_BEM.articles = new List<ArticleModel>();
            var articles = blogService.GetArticlesForBlog(blog_id);
            foreach(var a in articles)
            {
                ArticleModel my_article = new ArticleModel();
                my_article.ArticleId = a.ArticleId;
                my_article.BlogId = a.BlogId;
                my_article.BlogName = new_BEM.BlogName;
                my_article.ImagesPaths = a.ImagePaths;
                my_article.text = a.text;
                my_article.title = a.title;
                my_article.UserId = user_id;
                my_article.Status_of_ArticleId = a.Status_of_ArticleId;
                my_article.comments = new List<Tuple<string, string,string, DateTime, int>>();
                foreach(var comment in a.Comments)
                {
                    Tuple<string, string,string, DateTime, int> comment_art = new Tuple<string,string, string, DateTime, int>(comment.image_path, comment.user_name, comment.text, comment.datetime, comment.status_id);
                    my_article.comments.Add(comment_art);
                }
                new_BEM.articles.Add(my_article);
            }
            ViewBag.UserId = user_id;
            ViewBag.UserNick = blogService.GetUserNicknameForBlog(blog_id);
            ViewBag.Categories = blogService.GetAllCategories();
            return View(new_BEM);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditDescription(int blog_id, string text, BlogEditModel model)
        {
            if (ModelState.IsValid)
            {
                BlogDTO blogDTO = new BlogDTO();
                blogDTO.Id = model.BlogId;
                blogDTO.blogDescription = model.BlogDescription;
                blogDTO.BlogName = model.BlogName;
                blogDTO.CategoryId = blogService.GetAllCategories().Where(x => x.category_name == model.CategoryName).FirstOrDefault().CategoryId;
                blogDTO.date_of_last_update = DateTime.Now;
                blogService.EditDescription(blogDTO);
                return RedirectToAction("EditBlog", new { blog_id = blog_id });
            }
            else { return RedirectToAction("EditBlog", new { blog_id = blog_id }); }
        }
        [HttpGet]
        public ActionResult AddArticle(BlogEditModel BEM)
        {
            return RedirectToAction("AddArticle", "Article", new { blogname = BEM.BlogName, blog_id =BEM.BlogId, user_id = blogService.GetUserIdForBlog(BEM.BlogId)});
        }
        [HttpGet]
        public ActionResult DeleteArticle(int blog_id)
        {
            return RedirectToAction("EditBlog", "Blog", new { blog_id = blog_id, user_id = blogService.GetUserIdForBlog(blog_id) });
        }
        [HttpPost]
        public ActionResult DeleteBlog(int blog_id, string user_id)
        {
            blogService.Delete(blog_id);
            return RedirectToAction("Index", "Blog", new { UserId = user_id, UserNick = blogService.GetUserById(user_id).nickname });
        }

        public ActionResult SeeAllBlogs()
        {
            List<BlogModel> allBlogs = new List<BlogModel>();
            var blogs = blogService.GetAll();
            foreach(var blog in blogs)
            {
                BlogModel blogModel = new BlogModel { Id=blog.Id,
                BlogDescription=blog.blogDescription,
                Blogname=blog.BlogName,
                CategoryName = blogService.GetCategoryForBlog(blog.Id).category_name,
                UserId = blog.UserId,
                UserNickname=blogService.GetUserById(blog.UserId).nickname,
                date_of_last_update=blog.date_of_last_update
                };
                allBlogs.Add(blogModel);
            }
            ViewBag.UserEmail = blogService.GetUserByName(User.Identity.Name).Email;
            return View(allBlogs);

        }

        //[HttpPost]
        public ActionResult SeeBlog(int blog_id)
        {
            BlogEditModel new_BEM = new BlogEditModel();
            string user_id = blogService.GetUserIdForBlog(blog_id);
            new_BEM.BlogName = blogService.GetAllForUser(user_id).Where(x => x.Id == blog_id).First().BlogName;
            new_BEM.BlogId = blog_id;
            new_BEM.BlogDescription = blogService.GetAllForUser(user_id).Where(x => x.Id == blog_id).First().blogDescription;
            new_BEM.articles = new List<ArticleModel>();
            var articles = blogService.GetArticlesForBlog(blog_id);
            foreach (var a in articles)
            {
                ArticleModel my_article = new ArticleModel();
                my_article.ArticleId = a.ArticleId;
                my_article.BlogId = a.BlogId;
                my_article.BlogName = new_BEM.BlogName;
                my_article.ImagesPaths = a.ImagePaths;
                my_article.text = a.text;
                my_article.title = a.title;
                my_article.UserId = user_id;
                my_article.Status_of_ArticleId = a.Status_of_ArticleId;
                my_article.comments = new List<Tuple<string, string, string, DateTime, int>>();
                foreach (var comment in a.Comments)
                {
                    Tuple<string, string, string, DateTime, int> comment_art = new Tuple<string, string, string, DateTime, int>(comment.image_path, comment.user_name, comment.text, comment.datetime, comment.status_id);
                    my_article.comments.Add(comment_art);
                }
                new_BEM.articles.Add(my_article);
            }
            ViewBag.UserId = user_id;
            ViewBag.UserNick = blogService.GetUserNicknameForBlog(blog_id);
            return View(new_BEM);
        }

        [HttpPost]
        public ActionResult SearchByTags(string all_tags, int blog_id=0)
        {
            List<ArticleDTO> articles_with_tags = blogService.GetArticlesWithTags(all_tags, null, blog_id).ToList();
            List<ArticleModel> articlesmodels_with_tags = new List<ArticleModel>();
            foreach(var article in articles_with_tags)
            {
                if (article.Status_of_ArticleId == 1)
                {
                    ArticleModel art_model = new ArticleModel();
                    art_model.ArticleId = article.ArticleId;
                    art_model.BlogId = article.BlogId;
                    art_model.BlogName = blogService.GetAll().Where(x => x.Id == article.BlogId).First().BlogName;
                    foreach (var comment in article.Comments)
                    {
                        Tuple<string, string, string, DateTime, int> comment_art = new Tuple<string, string, string, DateTime, int>(comment.image_path, comment.user_name, comment.text, comment.datetime, comment.status_id);
                        art_model.comments.Add(comment_art);
                    }
                    art_model.ImagesPaths = article.ImagePaths;
                    art_model.text = article.text;
                    art_model.title = article.title;
                    art_model.UserId = blogService.GetUserIdForBlog(article.BlogId);
                    art_model.Status_of_ArticleId = article.Status_of_ArticleId;
                    articlesmodels_with_tags.Add(art_model);
                }
            }
            return View("SeeArticles",articlesmodels_with_tags);
        }

        public ActionResult GoToUserProfile(string user_id)
        {
            return RedirectToAction("ClientProfile","Account",new { Email = blogService.GetUserById(user_id).Email});
        }
    }
}