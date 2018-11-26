using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Malden.Portal.Data.TableStorage.ProductSerialNumbers
{
    public class ProductSerialNumberRepository : IProductCatalogueRepository
    {
        private readonly CloudTableClient _client;

        public ProductSerialNumberRepository()
        {
            var connectionString = ConnectionStringBuilder.GetConnectionString;

            Utilities.Start(connectionString);

            var storageAccount = CloudStorageAccount.FromConfigurationSetting(connectionString);

            _client = storageAccount.CreateCloudTableClient();
        }

        public void Add(IProductCatalogue productSerialNumber)
        {
            var serialNumberEntity = new ProductSerialNumberEntity
            {
                Id = productSerialNumber.Id,
                ProductId = productSerialNumber.ProductId,
                SerialNumber = productSerialNumber.SerialNumber,
                CurrentReleaseId = productSerialNumber.CurrentReleaseId,
                RowKey = KeyFactory.RowKey(productSerialNumber.Id),
                PartitionKey = KeyFactory.PartitionKey(productSerialNumber.SerialNumber.ToString()),
                Timestamp = DateTime.Now
            };

            var context = _client.GetDataServiceContext();

            context.AddObject(Tables.ProductSerialNumbers, serialNumberEntity);
            context.SaveChanges();
        }

        public void Update(IProductCatalogue productSerialNumber)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var serialNumberEntity = (from p in context.CreateQuery<ProductSerialNumberEntity>(Tables.ProductSerialNumbers)
                                      where p.SerialNumber == productSerialNumber.SerialNumber
                                      select p).FirstOrDefault();

            if (serialNumberEntity != null)
            {
                serialNumberEntity.Id = productSerialNumber.Id;
                serialNumberEntity.ProductId = productSerialNumber.ProductId;
                serialNumberEntity.SerialNumber = productSerialNumber.SerialNumber;
                serialNumberEntity.CurrentReleaseId = productSerialNumber.CurrentReleaseId;
                serialNumberEntity.RowKey = KeyFactory.RowKey(productSerialNumber.Id);
                serialNumberEntity.PartitionKey = KeyFactory.PartitionKey(productSerialNumber.SerialNumber.ToString());
                serialNumberEntity.Timestamp = DateTime.Now;

                //context.Detach(serialNumberEntity);
                //context.Detach(productSerialNumber);
                //context.AttachTo(Tables.ProductSerialNumbers, serialNumberEntity);
                context.UpdateObject(serialNumberEntity);
                context.SaveChanges();
            }
        }

        public void Delete(int serialNumber)
        {
            var serialNumberEntity = GetProductDetailsBySerialNumber(serialNumber);

            if (serialNumberEntity != null)
            {
                var context = _client.GetDataServiceContext();
                context.AttachTo(Tables.ProductSerialNumbers, serialNumberEntity, "*");
                context.DeleteObject(serialNumberEntity);
                context.SaveChanges();
            }
        }

        public IProductCatalogue GetProductDetailsBySerialNumber(int serialNumber)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);
            var results = (from serialNumebrs in context.CreateQuery<ProductSerialNumberEntity>(Tables.ProductSerialNumbers)
                           where (serialNumebrs.SerialNumber.Equals(serialNumber))
                           select serialNumebrs).FirstOrDefault();

            return results;
        }

        public bool IsRecordExists(int serialNumber)
        {
            return GetProductDetailsBySerialNumber(serialNumber) != null;
        }

        public bool IsReleaseHasProducts(string releaseId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var count = (from releases in context.CreateQuery<ProductSerialNumberEntity>(Tables.ProductSerialNumbers)
                         where (releases.CurrentReleaseId == releaseId)
                         select releases).ToList().Count();

            return count > 0;
        }

        public IList<IProductCatalogue> List()
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var serialNumbers = (from u in context.CreateQuery<ProductSerialNumberEntity>(Tables.ProductSerialNumbers)
                                 where u.SerialNumber >= 0
                                 select u).ToList<IProductCatalogue>();

            //var serialNumbers = ProductSerialNumberQuery(context).ToList<IProductSerialNumber>();

            return serialNumbers;
        }

        private CloudTableQuery<ProductSerialNumberEntity> ProductSerialNumberQuery(TableServiceContext context)
        {
            return context.CreateQuery<ProductSerialNumberEntity>(Tables.ProductSerialNumbers).AsTableServiceQuery();
        }

        public int HighestSerialNumber()
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var serialNumbers = ProductSerialNumberQuery(context).ToList<IProductCatalogue>();

            return serialNumbers.Count() > 0 ? serialNumbers.OrderBy(c => c.SerialNumber).LastOrDefault().SerialNumber : 0;
        }

        public IProductCatalogue Get(string serialKey)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            return (from u in context.CreateQuery<ProductSerialNumberEntity>(Tables.ProductSerialNumbers)
                    where u.Id == serialKey
                    select u).FirstOrDefault();
        }
    }
}