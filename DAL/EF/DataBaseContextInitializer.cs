using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DAL.Entities;

namespace DAL.EF
{
    class DataBaseContextInitializer : CreateDatabaseIfNotExists<DataBaseContext>
    {
        protected override void Seed(DataBaseContext db_context)
        {
            base.InitializeDatabase(db_context);
            db_context.SaveChanges();
        }
    }
}
