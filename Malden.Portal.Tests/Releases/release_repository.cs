using Malden.Portal.BLL;
using Malden.Portal.Data;
using Malden.Portal.Data.TableStorage;
using Malden.Portal.Data.TableStorage.Releases;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Modules;
using System;
using System.Linq;
using System.Text;

namespace Malden.Portal.Tests.Releases
{
    [TestClass]
    public class release_repository
    {
        private readonly StandardKernel _kernel = new StandardKernel();
        private IReleaseRepository _releaseRepository;
        private IProductRepository _productRepository;

        [TestInitialize]
        public void initialize()
        {
            var modules = new System.Collections.Generic.List<INinjectModule>
            {
                new DependencyResolver.DataLayer(),
                new DependencyResolver.LogicLayer()
            };

            _kernel.Load(modules);

            _releaseRepository = _kernel.Get<IReleaseRepository>();
            _productRepository = _kernel.Get<IProductRepository>();
        }

        [TestMethod]
        public void it_can_add_a_release()
        {
            var releaseLogic = _kernel.Get<IReleaseLogic>(); // new ReleaseLogic(_releaseRepository, new ReleaseEntity(), new MaintenanceContractRepository(), new ProductSerialNumberRepository(), new ProductRepository(), new FileManager(), new);

            var release = new Release("4.7.4.9") { DateOfRelease = DateTime.Now, ProductId = "2b1ee18c-916a-4a5d-ba56-bc1063dabb96", Version = new Version("4.7.4.9"), Id = Guid.NewGuid().ToString() };
            releaseLogic.Add(release);
        }

        [TestMethod]
        public void it_can_get_release()
        {
            var release = _releaseRepository.Get("07A067E0-935C-DF63-1E3F-B71A378C34F3");
            var versionInfo = new Version(release.Version);
            var product = _productRepository.GetById(release.ProductId);

            Assert.AreEqual("MultiDSLA", product.Name);
        }

        [TestMethod]
        public void it_can_get_the_latest_release()
        {
            var releaseLogic = _kernel.Get<IReleaseLogic>();//new ReleaseLogic(_releaseRepository, new ReleaseEntity(), new MaintenanceContractRepository(), new ProductSerialNumberRepository(), new ProductRepository(), new FileManager());

            var latestRelease = releaseLogic.LatestRelease(2348, "2b1ee18c-916a-4a5d-ba56-bc1063dabb96", false);

            Assert.AreEqual("4.8", latestRelease.Version.ToString());
        }

        [TestMethod]
        public void it_can_upload_a_file()
        {
            var fileManager = new FileManager();

            fileManager.Upload("dsla", @"C:\Users\ps\2.3.png");
        }

        [TestMethod]
        public void encrypt()
        {
            var encd = KeyFactory.PartitionKey("2.3");

            byte[] data = Convert.FromBase64String(encd);
            var decd = Encoding.Unicode.GetString(data);
        }

        [TestMethod]
        public void it_can_get_old_releases()
        {
            var releaseLogic = _kernel.Get<IReleaseLogic>(); //new ReleaseLogic(new ReleaseRepository(), new ReleaseEntity(), new MaintenanceContractRepository(), new ProductSerialNumberRepository(), new ProductRepository(), new FileManager());
            //var oldReleases = releaseLogic.OldReleases(2822);
            //Assert.AreEqual(2, oldReleases.Count);

            var currentRelease = releaseLogic.GetById("e111fd27-3b78-4b5e-90e1-0a4d17dc080c");
            var oldReleases = releaseLogic.ReleasesByTheProduct(currentRelease, false);
        }

        [TestMethod]
        public void it_can_get_old_relase_for_a_serial_numer()
        {
            //var serialNumber = 1988;
            var userPurchaseLogic = _kernel.Get<IUserPurchaseLogic>();

            var oldReleases = userPurchaseLogic.List("prabah@malden.co.uk1");

            var topReleases = (from u in oldReleases
                               where u.SerialNumber == 1988
                               select u.OldReleases).ToList();

            Assert.AreEqual(2, topReleases.Count);
        }

        [TestMethod]
        public void it_can_get_only_the_major_releases()
        {
            var releaseLogic = _kernel.Get<IReleaseLogic>();
            var allReleases = releaseLogic.OldReleasesByDate(1423);
        }
    }
}