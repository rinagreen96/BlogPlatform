using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private DataBaseContext db;
        private ArticleRepository articleRepository;
        private BlogRepository blogRepository;
        private CategoryRepository categoryRepository;
        private CommentRepository commentRepository;
        private ImageRepository imageRepository;
        private Status_of_ArticleRepository status_of_articleRepository;
        private Status_of_CommentRepository status_of_commentRepository;
        private TagRepository tagRepository;

        private ApplicationUserManager userManager;
        private ApplicationRoleManager roleManager;
        private IClientManager clientManager;

        public EFUnitOfWork()
        {
            db = new DataBaseContext();
            userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db));
            clientManager = new ClientManager(db);
        }
        public IRepository<Article> Articles
        {
            get
            {
                if (articleRepository == null)
                    articleRepository = new ArticleRepository(db);
                return articleRepository;
            }
        }

        public IRepository<Blog> Blogs
        {
            get
            {
                if (blogRepository == null)
                    blogRepository = new BlogRepository(db);
                return blogRepository;
            }
        }

        public IRepository<Category> Categories
        {
            get
            {
                if (categoryRepository == null)
                    categoryRepository = new CategoryRepository(db);
                return categoryRepository;
            }
        }

        public IRepository<Comment> Comments
        {
            get
            {
                if (commentRepository == null)
                    commentRepository = new CommentRepository(db);
                return commentRepository;
            }
        }

        public IRepository<Image> Images
        {
            get
            {
                if (imageRepository == null)
                    imageRepository = new ImageRepository(db);
                return imageRepository;
            }
        }

        public IRepository<Status_of_Article> Status_of_Articles
        {
            get
            {
                if (status_of_articleRepository == null)
                    status_of_articleRepository = new Status_of_ArticleRepository(db);
                return status_of_articleRepository;
            }
        }

        public IRepository<Status_of_Comment> Status_of_Comments
        {
            get
            {
                if (status_of_commentRepository == null)
                    status_of_commentRepository = new Status_of_CommentRepository(db);
                return status_of_commentRepository;
            }
        }

        public IRepository<Tag> Tags
        {
            get
            {
                if (tagRepository == null)
                    tagRepository = new TagRepository(db);
                return tagRepository;
            }
        }

        public ApplicationUserManager UserManager
        {
            get { return userManager; }
        }

        public IClientManager ClientManager
        {
            get { return clientManager; }
        }

        public ApplicationRoleManager RoleManager
        {
            get { return roleManager; }
        }

        public void Save()
        {
            db.SaveChanges();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);//informing GC that this class was cleaned up fully
        }
    }
}
