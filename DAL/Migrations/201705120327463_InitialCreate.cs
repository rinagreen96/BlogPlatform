namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        title = c.String(),
                        text = c.String(),
                        BlogId = c.Int(),
                        Status_of_ArticleId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Blogs", t => t.BlogId)
                .ForeignKey("dbo.Status_of_Article", t => t.Status_of_ArticleId)
                .Index(t => t.BlogId)
                .Index(t => t.Status_of_ArticleId);
            
            CreateTable(
                "dbo.Blogs",
                c => new
                    {
                        BlogId = c.Int(nullable: false, identity: true),
                        BlogName = c.String(),
                        BlogDescription = c.String(),
                        date_of_creation = c.DateTime(),
                        date_of_last_update = c.DateTime(),
                        ApplicationUserId = c.String(maxLength: 128),
                        CategoryID = c.Int(),
                    })
                .PrimaryKey(t => t.BlogId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.Categories", t => t.CategoryID)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.CategoryID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ClientProfiles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        sername = c.String(),
                        name = c.String(),
                        password = c.String(),
                        nickname = c.String(),
                        date_of_registration = c.DateTime(nullable: false),
                        city = c.String(),
                        avatar_path = c.String(),
                        info = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        text = c.String(),
                        date_of_comment = c.DateTime(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                        Status_of_CommentId = c.Int(),
                        ArticleId = c.Int(),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.Articles", t => t.ArticleId)
                .ForeignKey("dbo.Status_of_Comment", t => t.Status_of_CommentId)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.Status_of_CommentId)
                .Index(t => t.ArticleId);
            
            CreateTable(
                "dbo.Status_of_Comment",
                c => new
                    {
                        Status_of_CommentId = c.Int(nullable: false, identity: true),
                        status_of_comment_name = c.String(),
                    })
                .PrimaryKey(t => t.Status_of_CommentId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        category_name = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        ImageId = c.Int(nullable: false, identity: true),
                        ArticleId = c.Int(),
                        image_path = c.String(),
                    })
                .PrimaryKey(t => t.ImageId)
                .ForeignKey("dbo.Articles", t => t.ArticleId)
                .Index(t => t.ArticleId);
            
            CreateTable(
                "dbo.Status_of_Article",
                c => new
                    {
                        Status_of_ArticleId = c.Int(nullable: false, identity: true),
                        status_of_article_name = c.String(),
                    })
                .PrimaryKey(t => t.Status_of_ArticleId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.TagArticles",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Article_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Article_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.Articles", t => t.Article_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Article_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.TagArticles", "Article_Id", "dbo.Articles");
            DropForeignKey("dbo.TagArticles", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.Articles", "Status_of_ArticleId", "dbo.Status_of_Article");
            DropForeignKey("dbo.Images", "ArticleId", "dbo.Articles");
            DropForeignKey("dbo.Blogs", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.Articles", "BlogId", "dbo.Blogs");
            DropForeignKey("dbo.Blogs", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Comments", "Status_of_CommentId", "dbo.Status_of_Comment");
            DropForeignKey("dbo.Comments", "ArticleId", "dbo.Articles");
            DropForeignKey("dbo.Comments", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ClientProfiles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.TagArticles", new[] { "Article_Id" });
            DropIndex("dbo.TagArticles", new[] { "Tag_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Images", new[] { "ArticleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Comments", new[] { "ArticleId" });
            DropIndex("dbo.Comments", new[] { "Status_of_CommentId" });
            DropIndex("dbo.Comments", new[] { "ApplicationUserId" });
            DropIndex("dbo.ClientProfiles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Blogs", new[] { "CategoryID" });
            DropIndex("dbo.Blogs", new[] { "ApplicationUserId" });
            DropIndex("dbo.Articles", new[] { "Status_of_ArticleId" });
            DropIndex("dbo.Articles", new[] { "BlogId" });
            DropTable("dbo.TagArticles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Tags");
            DropTable("dbo.Status_of_Article");
            DropTable("dbo.Images");
            DropTable("dbo.Categories");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Status_of_Comment");
            DropTable("dbo.Comments");
            DropTable("dbo.ClientProfiles");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Blogs");
            DropTable("dbo.Articles");
        }
    }
}
