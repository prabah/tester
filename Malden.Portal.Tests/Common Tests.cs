using Malden.Portal.BLL;
using Malden.Portal.BLL.Utilities;
using Malden.Portal.Data;
using Malden.Portal.Data.TableStorage;
using Malden.Portal.Service.WebNew;
using Ninject;
using Ninject.Modules;
using NUnit.Framework;
using System;

namespace Malden.Portal.Tests
{
    public class Common_Tests
    {
        private readonly StandardKernel _kernel = new StandardKernel();

        [SetUp]
        public void setup()
        {
            var modules = new System.Collections.Generic.List<INinjectModule>
            {
                new DependencyResolver.DataLayer(),
                new DependencyResolver.LogicLayer()
            };

            _kernel.Load(modules);
        }

        [Test]
        public void it_can_send_email()
        {
            Emailer.SendEmail();
        }

        [Test]
        public void it_can_validate_a_serial_number_exists()
        {
            var ms = new MaldenService();
            var output = ms.IsSerialNumberExists(1, "prabah@malden.co.uk1", "Testtest1!");
        }

        [Test]
        public void it_can_highest_serial_number()
        {
            var ms = new MaldenService();
            var output = ms.HighestSerialNumber();
        }

        [Test]
        public void it_can_update_all_releases()
        {
            var releaseLogic = _kernel.Get<IReleaseLogic>();

            var releasess = releaseLogic.List();

            foreach (Release r in releasess)
            {
                r.IsHidden = false;
                releaseLogic.Update(r);
            }
        }

        [Test]
        public void it_can_update_all_products()
        {
            var productLogic = _kernel.Get<IProductLogic>();
            var products = productLogic.List();

            foreach (Product p in products)
            {
                p.IsMaintained = false;
                productLogic.Update(p);
            }
        }

        [Test]
        public void it_can_update_maintenance_contract()
        {
            var mcLogic = _kernel.Get<IMaintenanceContractLogic>();
            var productSerialNumberLogic = _kernel.Get<IProductCatalogueLogic>();
            mcLogic.Update(new MaintenanceContract { DateOfExpiry = new System.DateTime(2012, 12, 31), SerialKeyId = productSerialNumberLogic.GetIdBySerialNumber(2844) });
        }

        [Test]
        public void it_can_update_a_user_purchase()
        {
            var userPurchaseLogic = _kernel.Get<IUserPurchaseLogic>();
            var productLogic = _kernel.Get<IProductLogic>();
            var serialLogic = _kernel.Get<IProductCatalogueLogic>();

            var productSerialNumber = new ProductCatalogue { SerialNumber = 2844, CurrentRelease = "2.7.1.0", ProductId = productLogic.GetById(3).Id, Id = serialLogic.GetIdBySerialNumber(2844) };
            userPurchaseLogic.Update(new UserPurchase { SerialNumber = 2844, ProductId = productSerialNumber.ProductId, UserId = "prabah@@malden.co.uk1" }, "prabah@malden.co.uk1");
        }

        [Test]
        public void it_can_update_a_serial_number()
        {
            var serialLogic = _kernel.Get<IProductCatalogueLogic>();
            var productLogic = _kernel.Get<IProductLogic>();

            var productSerialNumber = new ProductCatalogue { SerialNumber = 2812, CurrentRelease = "2.7.1.0", ProductId = productLogic.GetById(3).Id, Id = serialLogic.GetIdBySerialNumber(2844) };
            serialLogic.Update(productSerialNumber);

            var updateDetails = serialLogic.GetByKey(2844);
            //Assert.AreEqual(updateDetails, productSerialNumber);
        }

        [Test]
        public void it_can_decode_an_MD5()
        {
            var md5 = PasswordResolver.CalculateMD5Hash("2837");
        }

        private string MD5(string serial)
        {
            return PasswordResolver.CalculateMD5Hash(serial).Substring(0, 5);
        }

        [Test]
        public void it_can_validate_customer_input()
        {
            var userPurchaseLogic = _kernel.Get<IUserPurchaseLogic>();
            var output = userPurchaseLogic.IsValidSerialNumber("prabah@malden.co.uk1", "03636" + "-" + MD5("3636"));
        }

        [Test]
        public void it_can_delete_inactive_users()
        {
            var userLogic = _kernel.Get<IUserLogic>();

            userLogic.DeleteInactiveUsers((int)Malden.Portal.BLL.User.UserType.Customer);
        }

        

        [Test]
        public void it_can_activate_an_user()
        {
            var userlLogic = _kernel.Get<IUserLogic>();
            userlLogic.ActivateUser("ea89d92e6b034d33b00edf7ba4ec4d1b", 1);
        }



        [Test]
        public void it_can_add_user_email_record()
        {


            var uniqueId = Guid.NewGuid().ToString("N");

            var emailManager = _kernel.Get<IEmailManager>();
            var emailRepository = _kernel.Get<IEmailManagerRepository>();
            var userRepository = _kernel.Get<IUserRepository>();
            var userId = "a4242940-17a9-42c4-99d7-f7baeee5fb14";
            var user = userRepository.Get(userId);

            emailManager.Id = uniqueId;
            emailManager.UserId = userId;
            
            emailRepository.Add(emailManager);
        }

        [Test]
        public void it_can_validate_an_invalid_serial_number()
        {
            var purchaseLogic = _kernel.Get<IUserPurchaseLogic>();
            Assert.IsFalse(purchaseLogic.IsValidSerialNumber("prabah@gmail.com", "02951-02180"));
             
        }

        [Test]
        public void it_can_strip_serial_and_releaseId()
        {
            var dataId = "2951~2#b610fce0-b18e-4374-bfa1-e7a47bb4070c";

            var indexOfTilda = dataId.IndexOf("~");
            var indexOfHash = dataId.IndexOf("#");

            var serial = dataId.Substring(0, indexOfTilda);
            var releaseId = dataId.Substring(indexOfHash + 1, 36);
            var fileType = dataId.Substring(indexOfTilda+1, indexOfHash - (indexOfTilda+1));
        }

        [Test]
        public void it_can_find_user_encrypted_key()
        {
            var key = "89B3BD33-32B2-4F56-B44F-720B84E581AB";
            var id = KeyFactory.PartitionKey(key);
            var td = id + "";
        }

        [Test]
        public void it_can_create_partition_key()
        {

            var partitionKey = KeyFactory.PartitionKey("3085");
            Console.Out.WriteLine(partitionKey);
        }
 
    }
}