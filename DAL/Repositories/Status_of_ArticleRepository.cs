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
    public class Status_of_ArticleRepository : IRepository<Status_of_Article>
    {
        private DataBaseContext db;

        public Status_of_ArticleRepository(DataBaseContext context)
        {
            this.db = context;
        }

        public IEnumerable<Status_of_Article> GetAll()
        {
            return db.Status_of_Articles;
        }

        public Status_of_Article Get(int id)
        {
            return db.Status_of_Articles.Find(id);
        }

        public void Create(Status_of_Article status_of_article)
        {
            db.Status_of_Articles.Add(status_of_article);
        }

        public void Update(Status_of_Article status_of_article)
        {
            db.Entry(status_of_article).State = EntityState.Modified;
        }

        public IEnumerable<Status_of_Article> Find(Func<Status_of_Article, Boolean> predicate)
        {
            return db.Status_of_Articles.Where(predicate).ToList();
        }

        public void Delete(int id)
        {
            Status_of_Article status_of_article = db.Status_of_Articles.Find(id);
            if (status_of_article != null)
                db.Status_of_Articles.Remove(status_of_article);
        }
    }
}



