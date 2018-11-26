using Malden.Portal.Data;

namespace Malden.Portal.Tests.ProductSerialNumbers
{
    public class TestProductSerialNumber : IProductCatalogue
    {
        public string Id { get; set; }

        public string ProductId { get; set; }

        public int SerialNumber { get; set; }

        public string CurrentReleaseId { get; set; }
    }
}