using Malden.Portal.DependencyResolver;
using Ninject;
using Ninject.Modules;

namespace WebJobs
{
    internal class WebjobDependencyResolver : NinjectModule
    {
        private readonly IKernel _kernel;

        public WebjobDependencyResolver(StandardKernel kernel)
        {
            _kernel = kernel;
        }

        public void RegisterServices(IKernel kernel)
        {
            var modules = new System.Collections.Generic.List<INinjectModule>
            {
                new DataLayer(),
                new LogicLayer()
            };

            
            kernel.Load(modules);
        }



        public override void Load()
        {
            RegisterServices(_kernel);
        }
    }
}
