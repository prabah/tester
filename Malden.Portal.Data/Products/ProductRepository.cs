using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Malden.Portal.Data.TableStorage.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly CloudTableClient _client;

        public ProductRepository()
        {
            var connectionString = ConnectionStringBuilder.GetConnectionString;
            Utilities.Start(connectionString);

            var storageAccount = CloudStorageAccount.FromConfigurationSetting(connectionString);

            _client = storageAccount.CreateCloudTableClient();
        }

        public void Add(IProduct product)
        {
            var softwareEntity = new ProductEntity
            {
                Id = product.Id,
                Name = product.Name,
                FulfilmentId = product.FulfilmentId,
                PartitionKey = KeyFactory.PartitionKey(product.Id),
                ContainerName = product.ContainerName.ToLower(),
                RowKey = KeyFactory.RowKey(product.Name),
                Timestamp = DateTime.Now,
                IsMaintained = product.IsMaintained
            };

            var context = _client.GetDataServiceContext();
            context.AddObject(Tables.Products, softwareEntity);
            context.SaveChanges();
        }

        public IProduct GetById(string id)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            return (from u in context.CreateQuery<ProductEntity>(Tables.Products)
                    where u.Id == id
                    select u).FirstOrDefault();
        }

        public IList<IProduct> List()
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            return (from u in context.CreateQuery<ProductEntity>(Tables.Products)
                    select u).ToList<IProduct>().OrderBy(p => p.Name).ToList();
        }

        public void Delete(string id)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var product = (from u in context.CreateQuery<ProductEntity>(Tables.Products)
                           where u.Id == id
                           select u).FirstOrDefault();

            if (product != null) context.DeleteObject(product);
        }

        public void Update(IProduct product)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var productEntity = (from u in context.CreateQuery<ProductEntity>(Tables.Products)
                                 where u.Id == product.Id
                                 select u).FirstOrDefault();

            if (productEntity != null)
            {
                productEntity.Name = product.Name;
                productEntity.PartitionKey = KeyFactory.PartitionKey(product.Id);
                productEntity.FulfilmentId = product.FulfilmentId;
                productEntity.ContainerName = product.ContainerName;
                productEntity.IsMaintained = product.IsMaintained;
                productEntity.RowKey = KeyFactory.RowKey(product.Name);
                productEntity.Timestamp = DateTime.Now;
                //context.AttachTo(Tables.Products, productEntity);
                context.UpdateObject(productEntity);
                context.SaveChanges();
            }
            else
            {
                throw new ArgumentNullException("product", "The entry cannot be found !");
            }
        }

        public string ConvertToPortalProductId(int fulfilmentProductId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var firstOrDefault = (from u in context.CreateQuery<ProductEntity>(Tables.Products)
                                  where u.FulfilmentId == fulfilmentProductId
                                  select u).FirstOrDefault();
            return firstOrDefault != null ? firstOrDefault.Id : null;
        }

        public IProduct GetByFulfilmentId(int fulfilmentId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            return (from u in context.CreateQuery<ProductEntity>(Tables.Products)
                    where u.FulfilmentId == fulfilmentId
                    select u).FirstOrDefault();
        }

        public void Delete(IProduct product)
        {
            var productEntity = (ProductEntity)GetById(product.Id);

            if (product != null)
            {
                var context = _client.GetDataServiceContext();
                context.AttachTo(Tables.Products, productEntity, "*");
                context.DeleteObject(productEntity);
                context.SaveChanges();
            }
        }

        public bool IsAnyReleasesForProduct(string productId)
        {
            throw new NotImplementedException();
        }
    }
}