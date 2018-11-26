using Malden.Portal.BLL;
using Malden.Portal.Data;
using Malden.Portal.Data.TableStorage;
using Malden.Portal.Data.TableStorage.UserPurchases;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Modules;
using System;

namespace Malden.Portal.Tests.UserPurchases
{
    [TestClass]
    public class test_user_purchase_repository_should
    {
        private readonly StandardKernel _kernel = new StandardKernel();
        private IUserPurchaseRepository _userPurchaseRepository;
        private IUserPurchaseLogic _userPurchaseLogic;

        [TestInitialize]
        public void Setup()
        {
            _userPurchaseRepository = new UserPurchaseRepository();

            var modules = new System.Collections.Generic.List<INinjectModule>
            {
                new DependencyResolver.DataLayer(),
                new DependencyResolver.LogicLayer()
            };

            _kernel.Load(modules);

            _userPurchaseLogic = _kernel.Get<IUserPurchaseLogic>();

        }

        [TestMethod]
        public void add_user_purchase()
        {
            _userPurchaseRepository = new UserPurchaseRepository();
            var userPurchase = new UserPurchaseEntity
                                   {
                                       RegistrationCode = "2350F-NSDFK-LJWWK-LRWJN-S2350",
                                       ProductId = "2b1ee18c-916a-4a5d-ba56-bc1063dabb96",
                                       UserId = "prabah@malden.co.uk",
                                       SerialNumber = 2350
                                   };

            _userPurchaseRepository.Add(userPurchase, Guid.NewGuid().ToString());
        }

        [TestMethod]
        public void get_all_user_purchases()
        {
            //_userPurchaseRepository = new UserPurchaseRepository();
            //var userPurchases = _userPurchaseRepository.UserPurchasesByEmail("prabaah@gmail.com");
            var modules = new System.Collections.Generic.List<INinjectModule>
            {
                new DependencyResolver.DataLayer(),
                new DependencyResolver.LogicLayer()
            };

            _kernel.Load(modules);

            _userPurchaseLogic = _kernel.Get<IUserPurchaseLogic>();
            var userLogic = _kernel.Get<IUserLogic>();
            var userRepository = _kernel.Get<IUserRepository>();

            var users = userRepository.List();

            foreach (IUser iu in users)
            {

                var email = iu.Email;

                var userPurchases = _userPurchaseLogic.List(email);
                var user = userLogic.GetByEmail(email);
                var partitionKey = KeyFactory.PartitionKey(email);


                foreach (UserPurchase u in userPurchases)
                {
                    if (u.SerialNumber == 2932)
                    {
                        var cc = 1;
                        cc++;

                    }
                    u.UserId = user.Id;
                    _userPurchaseLogic.Update(u, email);
                }
            }
        }

        [TestMethod]
        public void get_all_user_purchases_by_serial()
        {
            _userPurchaseRepository = new UserPurchaseRepository();
            var userPurchases = _userPurchaseRepository.GetBySerialNumber(2350, "prabah@malden.co.uk1");
        }


        [TestMethod]
        public void latest_release_for_all_products()
        {
            var releases = _userPurchaseLogic.LatestReleasesForAllProducts();
        }
    }
}