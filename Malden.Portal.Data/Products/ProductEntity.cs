using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace Malden.Portal.Data.TableStorage.Products
{
    public class ProductEntity : TableServiceEntity, IProduct
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int FulfilmentId { get; set; }

        public string ContainerName { get; set; }

        public bool IsMaintained { get; set; }
    }
}