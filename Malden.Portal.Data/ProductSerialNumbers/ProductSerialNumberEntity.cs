using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace Malden.Portal.Data.TableStorage.ProductSerialNumbers
{
    public class ProductSerialNumberEntity : TableServiceEntity, IProductCatalogue
    {
        public string Id { get; set; }

        public string ProductId { get; set; }

        public int SerialNumber { get; set; }

        public string CurrentReleaseId { get; set; }
    }
}