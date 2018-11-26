using Malden.Portal.BLL;
using Malden.Portal.BLL.Utilities;
using Malden.Portal.Service.WebNew;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Modules;
using System;

namespace Malden.Portal.Service.Tests
{
    [TestClass]
    public class SerialKeyManager
    {
        private IMaldenService _gs;
        private readonly StandardKernel _kernel = new StandardKernel();
 

        [TestInitialize]
        public void Setup()
        {
            var modules = new System.Collections.Generic.List<INinjectModule>
            {
                new DependencyResolver.DataLayer(),
                new DependencyResolver.LogicLayer()
            };

            _kernel.Load(modules);
        }

        [TestMethod]
        public void it_can_validate_product_and_release()
        {
            IMaldenService gs = new MaldenService();
            var x = gs.IsValid(1, "4.0.3", "prabah@malden.co.uk", "Testtest1!");


        }

        [TestMethod]
        //[ExpectedException(typeof(System.IdentityModel.Tokens.SecurityTokenException))]
        public void it_can_add_a_maintenance()
        {
            IMaldenService gs = new MaldenService();

            var success = gs.AddMaintenance(16, 7025, new DateTime(2015, 7, 22), "1.0.3", 4134, "prabah@gmail.com", "Testtest1!", true);

        }


        [TestMethod]
        //[ExpectedException(typeof(System.IdentityModel.Tokens.SecurityTokenException))]
        public void it_can_add_a_product_without_maintenance()
        {
            IMaldenService gs = new MaldenService();

            var success = gs.AddProductWithoutMaintenance(1, 9999, "4.8.1", "prabah@malden.co.uk", "Testtest1!");

        }

        [TestMethod]
        public void it_can_update_a_maintenance()
        {
            IMaldenService gs = new MaldenService();

            var success = gs.UpdateMaintenanace(1, 1748, new DateTime(2014, 1, 12), "4.8.1", 29, "prabah@malden.co.uk", "Testtest1!");
        }

        [TestMethod]
        public void it_can_extend_a_maintenance()
        {
            IMaldenService gs = new MaldenService();

            var success = gs.ExtendMaintenanace(1, 8809, new DateTime(2013, 1, 12), 2222, "prabah@malden.co.uk", "Testtest1!");
        }

        [TestMethod]
        public void it_can_delete_inactive_users()
        {
            IMaldenService gs = new MaldenService();
            gs.DeleteInactive();
        }

        [TestMethod]
        public void it_can_add_a_mainatenance_contract()
        {
            _gs = new MaldenService();

            _gs.AddMaintenance(1, 2938, new DateTime(2014, 8, 20),"4.8.0",0, "prabah@gmail.com", "Testtest1!", false);
            //_gs.AddMaintenanaceContract("2818", new DateTime(2014, 8, 20), _userName, _pass);
            //_gs.AddMaintenanaceContract("2822", new DateTime(2013, 12, 15), _userName, _pass);
            //_gs.AddMaintenanaceContract("2824", new DateTime(2014, 3, 12), _userName, _pass);
            //_gs.AddMaintenanaceContract("2826", new DateTime(2013, 8, 20), _userName, _pass);
            //_gs.AddMaintenanaceContract("2829", new DateTime(2014, 5, 9), _userName, _pass);
            //_gs.AddMaintenanaceContract("2831", new DateTime(2013, 10, 11), _userName, _pass);
            //_gs.AddMaintenanaceContract("2837", new DateTime(2013, 9, 15), _userName, _pass);
            //_gs.AddMaintenanaceContract("2844", new DateTime(2014, 2, 3), _userName, _pass);
            //_gs.AddMaintenanaceContract("2846", new DateTime(2014, 1, 10), _userName, _pass);
            //_gs.AddMaintenanaceContract("2848", new DateTime(2014, 4, 12), _userName, _pass);
        }

        [TestMethod]
        public void it_can_get_a_version()
        {
            var version = new Version(4, 1);

            var x = version.ToString();

            //Assert.IsTrue(version < version_new);
        }

        [TestMethod]
        public void it_can_add_a_version()
        {
            var releaseLogic = _kernel.Get<IReleaseLogic>();  //new ReleaseLogic(new ReleaseRepository(), new ReleaseEntity(), new MaintenanceContractRepository(),new ProductSerialNumberRepository(), new ProductRepository() , new FileManager());
            var release = new Release("4.5.4") { Id = Guid.NewGuid().ToString(), DateOfRelease = DateTime.Now, ProductId = "c6c2a11a-08a5-42cf-bf41-b5a346a449d9" };
            releaseLogic.Add(release);
        }

        //[TestMethod]
        //public void it_can_get_the_patch()
        //{
        //    IReleaseRepository releaseRepository = new ReleaseRepository();
        //    releaseRepository.LatestPatchRelease( "ab0a8694-21dc-48d3-b22f-476985a60fc1", "084d91c5-eed7-4006-aac3-1ce6a0e83a64");
        //}

        [TestMethod]
        public void it_can_throw_an_error()
        {
            ErrorLogger.Log(new ArgumentNullException("test error", new AccessViolationException()));
        }
    }
}