﻿using System;
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
    public class TagRepository : IRepository<Tag>
    {
        private DataBaseContext db;

        public TagRepository(DataBaseContext context)
        {
            this.db = context;
        }

        public IEnumerable<Tag> GetAll()
        {
            return db.Tags;
        }

        public Tag Get(int id)
        {
            return db.Tags.Find(id);
        }

        public void Create(Tag tag)
        {
            db.Tags.Add(tag);
        }

        public void Update(Tag tag)
        {
            db.Entry(tag).State = EntityState.Modified;
        }

        public IEnumerable<Tag> Find(Func<Tag, Boolean> predicate)
        {
            return db.Tags.Where(predicate).ToList();
        }

        public void Delete(int id)
        {
            Tag tag = db.Tags.Find(id);
            if (tag != null)
                db.Tags.Remove(tag);
        }
    }
}

