using Malden.Portal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Malden.Portal.Tests.Releases
{
    public class TestReleaseRepository : IReleaseRepository
    {
        private readonly IList<TestRelease> _products = new List<TestRelease>();

        public TestReleaseRepository()
        {
            const string path = @"E:\Source Code\Malden.Portal.New\Malden.Portal.Tests\Data\Releases.xml";
            var doc = XDocument.Load(path);

            _products = (from product in doc.Descendants("Release")
                         let pId = product.Attribute("id")
                         where pId != null
                         let id = pId.Value
                         let productId = product.Attribute("productId")
                         where productId != null
                         let prodId = productId.Value
                         let majorVersion = product.Attribute("major")
                         where majorVersion != null
                         let major = majorVersion.Value
                         let minorVersion = product.Attribute("minor")
                         where minorVersion != null
                         let minor = minorVersion.Value
                         let buildVersion = product.Attribute("build")
                         where buildVersion != null
                         let build = buildVersion.Value
                         let revisionVersion = product.Attribute("revision")
                         where revisionVersion != null
                         let revision = revisionVersion.Value
                         let dateOfRelease = product.Attribute("dor")
                         where dateOfRelease != null
                         let dor = dateOfRelease.Value
                         select new TestRelease
                         {
                             Id = id,
                             Version = major + minor + build + revision
                         }).ToList<TestRelease>();
        }

        public void Add(IRelease product)
        {
            _products.Add(new TestRelease
            {
                DateOfRelease = product.DateOfRelease,
                Id = product.Id,

                ProductId = product.ProductId,
                Version = product.Version
            });
        }

        public IRelease Get(string id)
        {
            return _products.Where(c => c.Id == id).FirstOrDefault();
        }

        public string ReleaseId(string version, string productId)
        {
            throw new NotImplementedException();
        }

        public string ReleaseId(Version version, string productId)
        {
            throw new NotImplementedException();
        }

        public IRelease GetByVersion(string version, string productId)
        {
            throw new NotImplementedException();
        }

        public IList<string> Releases(string productId, string currentRelease)
        {
            throw new NotImplementedException();
        }

        public string LatestRelease(string productId)
        {
            throw new NotImplementedException();
        }

        public IRelease GetLatestRelease(string productId, string currentReleaseId)
        {
            throw new NotImplementedException();
        }

        public IList<IRelease> GetReleasesByProductId(string productId)
        {
            throw new NotImplementedException();
        }

        public IList<IRelease> List()
        {
            throw new NotImplementedException();
        }

        public void Update(IRelease release)
        {
            throw new NotImplementedException();
        }

        public void Delete(IRelease release)
        {
            throw new NotImplementedException();
        }

        public bool IsAnyReleasesForProduct(string productId)
        {
            throw new NotImplementedException();
        }

        public IRelease GetByTrimmedId(string trimmedId)
        {
            throw new NotImplementedException();
        }

        public string ReleaseConnectionKey()
        {
            throw new NotImplementedException();
        }
    }
}