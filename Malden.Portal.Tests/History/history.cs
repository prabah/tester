using Malden.Portal.BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Modules;

namespace Malden.Portal.Tests.History
{
    /// <summary>
    /// Summary description for history
    /// </summary>
    [TestClass]
    public class history
    {
        private readonly StandardKernel _kernel = new StandardKernel();

        public history()
        {
            var modules = new System.Collections.Generic.List<INinjectModule>
            {
                new DependencyResolver.DataLayer(),
                new DependencyResolver.LogicLayer()
            };

            _kernel.Load(modules);
        }

        [TestMethod]
        public void it_can_add_history()
        {
            var history = new BLL.History { SerialNumber = 2822, UserEmail = "prabah@malden.co.uk", Version = new System.Version("4.8") };
            var historyLogic = _kernel.Get<IHistoryLogic>();
            historyLogic.Add(history);
        }
    }
}