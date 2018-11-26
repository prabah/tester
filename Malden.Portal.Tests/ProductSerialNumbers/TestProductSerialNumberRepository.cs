using Malden.Portal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Malden.Portal.Tests.ProductSerialNumbers
{
    public class TestProductSerialNumberRepository : IProductCatalogueRepository
    {
        private readonly IList<IProductCatalogue> _ProductSerialNumbers = new List<IProductCatalogue>();

        public TestProductSerialNumberRepository()
        {
            const string path = @"E:\Source Code\Malden.Portal.New\Malden.Portal.Tests\Data\product-serials.xml";
            var doc = XDocument.Load(path);

            _ProductSerialNumbers = (from product in doc.Descendants("SerialNumber")
                                     let pId = product.Attribute("Id")
                                     where pId != null
                                     let id = pId.Value
                                     let productId = product.Attribute("ProductId")
                                     where productId != null
                                     let prodId = productId.Value
                                     let serialId = product.Attribute("SerailId")
                                     where serialId != null
                                     let key = Convert.ToInt32(serialId.Value)

                                     select new TestProductSerialNumber
                                     {
                                         Id = id,
                                         ProductId = prodId,
                                         SerialNumber = key
                                     }).ToList<IProductCatalogue>();
        }

        public void Add(IProductCatalogue productSerialNumber)
        {
            throw new NotImplementedException();
        }

        public void Update(IProductCatalogue productSerialNumber)
        {
            throw new NotImplementedException();
        }

        public void Delete(int serialNumber)
        {
            throw new NotImplementedException();
        }

        public IProductCatalogue GetProductDetailsBySerialNumber(int serialNumber)
        {
            return _ProductSerialNumbers.Where(c => c.SerialNumber == serialNumber).FirstOrDefault();
        }

        public bool IsRecordExists(int serialNumber)
        {
            throw new NotImplementedException();
        }

        public bool IsReleaseHasProducts(string releaseId)
        {
            throw new NotImplementedException();
        }

        public IList<IProductCatalogue> List()
        {
            throw new NotImplementedException();
        }

        public int HighestSerialNumber()
        {
            throw new NotImplementedException();
        }

        public IProductCatalogue Get(string serialKey)
        {
            throw new NotImplementedException();
        }
    }
}