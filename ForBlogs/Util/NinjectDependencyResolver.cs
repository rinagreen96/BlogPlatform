using System;
using BLL.Services;
using System.Web.Mvc;
using Ninject;
using System.Collections.Generic;
using BLL.Interfaces;

namespace ForBlogs.Util
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
        private void AddBindings()
        {
            kernel.Bind<IBlogService>().To<BlogService>();
            kernel.Bind<IArticleService>().To<ArticleService>();
            kernel.Bind<IUserService>().To<UserService>();
            kernel.Bind<IModeratorService>().To<ModeratorService>();
        }
    }
}