using Malden.Portal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Malden.Portal.Tests.UserPurchases
{
    public class TestUserPurchaseRepostory : IUserPurchaseRepository
    {
        private readonly IList<TestUserPurchase> _userPurchase = new List<TestUserPurchase>();

        public TestUserPurchaseRepostory()
        {
            const string path = @"E:\Source Code\Malden.Portal.New\Malden.Portal.Tests\Data\user_pruchases.xml";
            var doc = XDocument.Load(path);

            _userPurchase = (from product in doc.Descendants("userPurchase")
                             let pid = product.Attribute("Id")
                             where pid != null
                             let id = pid.Value
                             let user = product.Attribute("UserId")
                             where user != null
                             let userId = user.Value
                             let serialNumber = product.Attribute("SerialNumber")
                             where serialNumber != null
                             let serial = serialNumber.Value
                             let productId = product.Attribute("ProductId")
                             where productId != null
                             let prodId = productId.Value
                             let currentRelease = product.Attribute("CurrentReleaseId")
                             where currentRelease != null
                             let currentReleaseId = currentRelease.Value

                             select new TestUserPurchase
                             {
                                 Id = id,
                                 ProductId = prodId,
                                 CurrentReleaseId = currentReleaseId,
                                 RegistrationCode = serial,
                                 UserId = "prabah@malden.co.uk1"
                             }).ToList<TestUserPurchase>();
        }

        public void Add(IUserPurchase userPurchase, string id)
        {
            _userPurchase.Add(new TestUserPurchase
            {
                Id = id,
                ProductId = userPurchase.ProductId,
                RegistrationCode = userPurchase.RegistrationCode,
                UserId = userPurchase.RegistrationCode
            });
        }

        public IUserPurchase GetBySerialNumber(int serialNumber, string email)
        {
            return _userPurchase.Where(c => c.RegistrationCode == serialNumber.ToString() && c.UserId == email).FirstOrDefault();
        }

        public void Remove(string serialNumber)
        {
            throw new NotImplementedException();
        }

        public void Update(IUserPurchase userPurchase, string email)
        {
            throw new NotImplementedException();
        }

        public IList<IUserPurchase> UserPurchasesByEmail(string email)
        {
            return _userPurchase.ToList<IUserPurchase>().Where(p => p.UserId == email).ToList();
        }

        public IUserPurchase Get(string id)
        {
            throw new NotImplementedException();
        }

        public bool IsAnyProdustsRegistered(string productId)
        {
            throw new NotImplementedException();
        }

        public bool IsValidSerialNumber(string userId, int serialNumber)
        {
            throw new NotImplementedException();
        }
    }
}