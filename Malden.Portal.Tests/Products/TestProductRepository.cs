using Malden.Portal.Data;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Malden.Portal.Tests.Products
{
    public class TestProductRepository : IProductRepository
    {
        private readonly IList<TestProduct> _products = new List<TestProduct>();

        public TestProductRepository()
        {
            const string path = @"E:\Source Code\Malden.Portal.New\Malden.Portal.Tests\Data\products.xml";
            var doc = XDocument.Load(path);

            _products = (from product in doc.Descendants("product")
                         let productId = product.Attribute("id")
                         where productId != null
                         let prodId = productId.Value
                         let prodName = product.Attribute("name")
                         where prodName != null
                         let name = prodName.Value
                         let pdescription = product.Attribute("description")
                         where pdescription != null
                         let description = pdescription.Value
                         select new TestProduct { Name = name, Id = prodId }).ToList<TestProduct>();
        }

        public void Add(IProduct software)
        {
            var testproduct = new TestProduct { Id = software.Id, Name = software.Name };
            _products.Add(testproduct);
        }

        public void Delete(string id)
        {
            var product = _products.Where<TestProduct>(t => t.Id == id).FirstOrDefault();
            int index = _products.IndexOf(product);

            _products.RemoveAt(index);
        }

        public void Update(IProduct software)
        {
            throw new System.NotImplementedException();
        }

        public IList<IProduct> List()
        {
            return _products.ToList<IProduct>();
        }

        public IProduct GetById(string id)
        {
            return _products.Where<TestProduct>(t => t.Id == id).FirstOrDefault();
        }

        public string ConvertToPortalProductId(int filfilmentProductId)
        {
            throw new System.NotImplementedException();
        }

        public IProduct GetByFulfilmentId(int fulfilmentId)
        {
            throw new System.NotImplementedException();
        }

        public bool IsAnyReleasesForProduct(string productId)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(IProduct product)
        {
            throw new System.NotImplementedException();
        }

        public string ContainerName(string productId)
        {
            throw new System.NotImplementedException();
        }
    }
}