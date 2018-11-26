[assembly: WebActivator.PreApplicationStartMethod(typeof(Malden.Portal.GUI.Azure.Webrole.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Malden.Portal.GUI.Azure.Webrole.App_Start.NinjectWebCommon), "Stop")]

namespace Malden.Portal.GUI.Azure.Webrole.App_Start
{
    using Malden.Portal.GUI.Azure.Webrole.Mailers;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Modules;
    using Ninject.Web.Common;
    using System;
    using System.Web;

    public static class NinjectWebCommon
    {
        public static IKernel kernelObject { get; set; }

        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            if (kernelObject != null) return kernelObject;

            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            kernel.Bind<IUserMailer>().To<UserMailer>();

            RegisterServices(kernel);
            kernelObject = kernel;
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            var modules = new System.Collections.Generic.List<INinjectModule>
            {
                new DependencyResolver.DataLayer(),
                new DependencyResolver.LogicLayer()
            };

            
            kernel.Load(modules);

            //RouteTable.Routes.Add(new ServiceRoute("SoftwareManager", new DependencyResolver.RestServiceFactory<ISoftwareManager>(), typeof(SoftwareManager)));
            //RouteTable.Routes.Add(new ServiceRoute("ReleaseManager", new DependencyResolver.RestServiceFactory<IReleaseManager>(), typeof(ReleaseManager)));
        }
    }
}