using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Malden.Portal.Data.TableStorage.UserPurchases
{
    public class UserPurchaseRepository : IUserPurchaseRepository
    {
        private readonly CloudTableClient _client;

        public UserPurchaseRepository()
        {
            var connectionString = ConnectionStringBuilder.GetConnectionString;

            Utilities.Start(connectionString);

            var storageAccount = CloudStorageAccount.FromConfigurationSetting(connectionString);

            _client = storageAccount.CreateCloudTableClient();
        }

        public void Add(IUserPurchase userPurchase, string id)
        {
            var userPurchaseEntity = new UserPurchaseEntity
            {
                Id = id,
                RegistrationCode = userPurchase.RegistrationCode,
                SerialNumber = userPurchase.SerialNumber,
                ProductId = userPurchase.ProductId,
                PartitionKey = KeyFactory.PartitionKey(userPurchase.UserId),
                RowKey = KeyFactory.RowKey(userPurchase.SerialNumber.ToString()),
                Timestamp = DateTime.Now,
                UserId = userPurchase.UserId
            };

            var context = _client.GetDataServiceContext();

            context.AddObject(Tables.UserPurchases, userPurchaseEntity);
            context.SaveChanges();
        }

        //TODO implement
        public void Remove(string serialNumber)
        {
            throw new NotImplementedException();
        }

        public void Update(IUserPurchase userPurchase, string email)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var userPurchaseEntity = (from u in context.CreateQuery<UserPurchaseEntity>(Tables.UserPurchases)
                                      where u.SerialNumber == userPurchase.SerialNumber && u.PartitionKey == KeyFactory.PartitionKey(email)
                                      select u).FirstOrDefault();

            if (userPurchaseEntity != null)
            {
                userPurchaseEntity.RegistrationCode = userPurchase.RegistrationCode;
                userPurchaseEntity.SerialNumber = userPurchase.SerialNumber;
                userPurchaseEntity.ProductId = userPurchase.ProductId;
                userPurchaseEntity.PartitionKey = KeyFactory.PartitionKey(userPurchase.UserId);
                userPurchaseEntity.RowKey = KeyFactory.RowKey(userPurchase.SerialNumber.ToString());
                userPurchaseEntity.Timestamp = DateTime.Now;
                userPurchaseEntity.UserId = userPurchase.UserId;

                //context.AttachTo(Tables.UserPurchases, userPurchaseEntity);
                context.UpdateObject(userPurchaseEntity);
                context.SaveChanges();
            }
        }

        public IList<IUserPurchase> UserPurchasesByEmail(string email)
        {
            var context = _client.GetDataServiceContext();

            return (from purchases in context.CreateQuery<UserPurchaseEntity>(Tables.UserPurchases)
                    where purchases.PartitionKey == KeyFactory.PartitionKey(email)
                    select purchases).ToList<IUserPurchase>();
        }

        public IUserPurchase GetBySerialNumber(int serialNumber, string email)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);


            var results = (from purchases in context.CreateQuery<UserPurchaseEntity>(Tables.UserPurchases)
                           where (purchases.PartitionKey == KeyFactory.PartitionKey(email) && purchases.SerialNumber == serialNumber)
                           select purchases).FirstOrDefault();

            return results;
        }

        public IUserPurchase Get(string id)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var results = (from purchases in context.CreateQuery<UserPurchaseEntity>(Tables.UserPurchases)
                           where (purchases.Id == id)
                           select purchases).FirstOrDefault();

            return results;
        }

        public bool IsValidSerialNumber(string email, int serialNumber)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var results = (from purchases in context.CreateQuery<UserPurchaseEntity>(Tables.UserPurchases)
                           where (purchases.PartitionKey == KeyFactory.PartitionKey(email) && purchases.SerialNumber == serialNumber)
                           select purchases).FirstOrDefault();

            return (results != null);
        }
    }
}