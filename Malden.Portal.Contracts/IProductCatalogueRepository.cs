using System.Collections.Generic;

namespace Malden.Portal.Data
{
    public interface IProductCatalogueRepository
    {
        void Add(IProductCatalogue productSerialNumber);

        void Update(IProductCatalogue productSerialNumber);

        void Delete(int serialNumber);

        IProductCatalogue GetProductDetailsBySerialNumber(int serialNumber);

        IProductCatalogue Get(string serialKey);

        bool IsRecordExists(int serialNumber);

        bool IsReleaseHasProducts(string releaseId);

        int HighestSerialNumber();

        IList<IProductCatalogue> List();
    }
}