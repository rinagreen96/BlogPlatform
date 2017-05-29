using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;
using DAL.Entities;
using DAL.EF;
using System.Data.Entity;

namespace DAL.Repositories
{
    public class BlogRepository : IRepository<Blog>
    {
        private DataBaseContext db;

        public BlogRepository(DataBaseContext context)
        {
            this.db = context;
        }

        public IEnumerable<Blog> GetAll()
        {
            return db.Blogs;
        }

        public Blog Get(int id)
        {
            return db.Blogs.Find(id);
        }

        public void Create(Blog blog)
        {
            db.Blogs.Add(blog);
        }

        public void Update(Blog blog)
        {
            db.Entry(blog).State = EntityState.Modified;
        }

        public IEnumerable<Blog> Find(Func<Blog, Boolean> predicate)
        {
            return db.Blogs.Where(predicate).ToList();
        }

        public void Delete(int id)
        {
            Blog blog = db.Blogs.Find(id);
            if (blog != null)
                db.Blogs.Remove(blog);
        }
    }
}

