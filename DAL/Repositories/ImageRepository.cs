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
    public class ImageRepository : IRepository<Image>
    {
        private DataBaseContext db;

        public ImageRepository(DataBaseContext context)
        {
            this.db = context;
        }

        public IEnumerable<Image> GetAll()
        {
            return db.Images;
        }

        public Image Get(int id)
        {
            return db.Images.Find(id);
        }

        public void Create(Image image)
        {
            db.Images.Add(image);
        }

        public void Update(Image image)
        {
            db.Entry(image).State = EntityState.Modified;
        }

        public IEnumerable<Image> Find(Func<Image, Boolean> predicate)
        {
            return db.Images.Where(predicate).ToList();
        }

        public void Delete(int id)
        {
            Image image = db.Images.Find(id);
            if (image != null)
                db.Images.Remove(image);
        }
    }
}


