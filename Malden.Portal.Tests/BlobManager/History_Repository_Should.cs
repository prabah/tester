using Malden.Portal.Data;
using Malden.Portal.Data.TableStorage.History;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Modules;
using System.Linq;

namespace Malden.Portal.Tests.BlobManager
{
    [TestClass]
    public class History_Repository_Should
    {
        private readonly StandardKernel _kernel = new StandardKernel();
        private IHistoryRepository _hisotryRepository;

        

        [TestInitialize]
        public void Setup()
        {
            

            var modules = new System.Collections.Generic.List<INinjectModule>
            {
                new DependencyResolver.DataLayer(),
                new DependencyResolver.LogicLayer()
            };

            _kernel.Load(modules);

            _hisotryRepository = new HistoryRepository();

        }

        [TestMethod]
        public void get_records_in_range()
        {
            _hisotryRepository = _kernel.Get<IHistoryRepository>();
            var downloads = _hisotryRepository.List(20, 20);
            var allDownloads = _hisotryRepository.List();
            var record_number_21 = allDownloads.Skip(20).Take(1).FirstOrDefault();


            Assert.AreEqual(20, downloads.Count());
            Assert.AreEqual(record_number_21.Id, downloads.FirstOrDefault().Id);
        }

        [TestMethod]
        public void get_total_number_of_records()
        {
            Assert.AreEqual(66, _hisotryRepository.TotalDownloads());
        }
        
    }
}

