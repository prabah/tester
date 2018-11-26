using Malden.Portal.Data;

namespace Malden.Portal.Tests.Products
{
    public class TestProduct : IProduct
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int FulfilmentId { get; set; }

        public string ContainerName { get; set; }

        public bool IsMaintained { get; set; }
    }
}