using Malden.Portal.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malden.Portal.Tests.ProductSerialNumbers
{
    [TestClass]
    public class product_serial_number_repository
    {
        private IProductCatalogueRepository _productSerialNumberRepository;

        [TestInitialize]
        public void initialize()
        {
            _productSerialNumberRepository = new TestProductSerialNumberRepository();
        }

        [TestMethod]
        public void it_can_add_a_serial_number()
        {
            //var serialNumberLogic = new ProductSerialNumberLogic(new ProductSerialNumberRepository(), new ProductSerialNumberEntity(), new ReleaseRepository());
            //serialNumberLogic.Add(new ProductSerialNumber { Id = Guid.NewGuid().ToString(), CurrentRelease = "4.8.2", ProductId = "1", SerialNumber = 9999 });
        }

        [TestMethod]
        public void it_can_get_by_key()
        {
            var productSerial = _productSerialNumberRepository.GetProductDetailsBySerialNumber(4691);

            Assert.AreEqual("4A916225-B064-3F27-7B0E-692982EDF073", productSerial.ProductId);
        }
    }
}