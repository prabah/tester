using Ninject;
using Ninject.Modules;

namespace Malden.Portal.Service.WebNew
{
    internal static class Bootstrapper
    {
        public static void RegisterServices(IKernel kernel)
        {
            var modules = new System.Collections.Generic.List<INinjectModule>
            {
                new DependencyResolver.DataLayer(),
                new DependencyResolver.LogicLayer()
            };

            kernel.Load(modules);
        }
    }
}