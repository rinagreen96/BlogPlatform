using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DAL.EF
{
    public class DataBaseContext: IdentityDbContext<ApplicationUser>
    {
        public DataBaseContext(): base("ForPBlog_DBConnection")
        {
            Database.SetInitializer<DataBaseContext>(new DropCreateDatabaseIfModelChanges<DataBaseContext>());
        }
        static DataBaseContext()
        {
            Database.SetInitializer<DataBaseContext>(new DataBaseContextInitializer());
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Status_of_Article> Status_of_Articles { get; set; }
        public DbSet<Status_of_Comment> Status_of_Comments { get; set; }
        public DbSet<ClientProfile> ClientProfiles { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
