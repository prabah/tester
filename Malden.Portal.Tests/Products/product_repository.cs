using Malden.Portal.BLL;
using Malden.Portal.Data;
using Malden.Portal.Data.TableStorage.Products;
using Malden.Portal.Data.TableStorage.Releases;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Malden.Portal.Tests.Products
{
    [TestClass]
    public class product_repository
    {
        private IProductRepository _productRepository;

        [TestInitialize]
        public void initialize()
        {
            _productRepository = new ProductRepository();
        }

        [TestMethod]
        public void add_a_product()
        {
            _productRepository.Add(new ProductEntity { Id = Guid.NewGuid().ToString(), Name = "Virtual Node", FulfilmentId = 2 });
            Assert.AreEqual(2, _productRepository.List().Count);
        }

        [TestMethod]
        public void it_can_convert_a_fulfilment_portalId()
        {
            var productLogic = new ProductLogic(new ProductRepository(), new ProductEntity(), new ReleaseRepository());
            var x = productLogic.ConvertToPortalProductId(5);
        }
    }
}