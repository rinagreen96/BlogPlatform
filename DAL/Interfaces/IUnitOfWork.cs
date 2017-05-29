using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Identity;

namespace DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Article> Articles { get; }
        IRepository<Blog> Blogs { get; }
        IRepository<Category> Categories { get; }
        IRepository<Comment> Comments { get; }
        IRepository<Image> Images { get; }
        IRepository<Status_of_Article> Status_of_Articles { get; }
        IRepository<Status_of_Comment> Status_of_Comments { get; }
        IRepository<Tag> Tags{ get; }
        ApplicationUserManager UserManager { get; }
        IClientManager ClientManager { get; }
        ApplicationRoleManager RoleManager { get; }
        // содержать ссылки на менеджеры пользователей и ролей, а также на репозиторий пользователей
        void Save();
    }
}
