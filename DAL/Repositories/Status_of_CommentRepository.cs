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
    public class Status_of_CommentRepository : IRepository<Status_of_Comment>
    {
        private DataBaseContext db;

        public Status_of_CommentRepository(DataBaseContext context)
        {
            this.db = context;
        }

        public IEnumerable<Status_of_Comment> GetAll()
        {
            return db.Status_of_Comments;
        }

        public Status_of_Comment Get(int id)
        {
            return db.Status_of_Comments.Find(id);
        }

        public void Create(Status_of_Comment status_of_comment)
        {
            db.Status_of_Comments.Add(status_of_comment);
        }

        public void Update(Status_of_Comment status_of_comment)
        {
            db.Entry(status_of_comment).State = EntityState.Modified;
        }

        public IEnumerable<Status_of_Comment> Find(Func<Status_of_Comment, Boolean> predicate)
        {
            return db.Status_of_Comments.Where(predicate).ToList();
        }

        public void Delete(int id)
        {
            Status_of_Comment status_of_comment = db.Status_of_Comments.Find(id);
            if (status_of_comment != null)
                db.Status_of_Comments.Remove(status_of_comment);
        }
    }
}



