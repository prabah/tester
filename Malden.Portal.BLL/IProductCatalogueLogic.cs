using System.Collections.Generic;

namespace Malden.Portal.BLL
{
    public interface IProductCatalogueLogic
    {
        void Add(ProductCatalogue productSerialNumber);

        void Delete(int serialNumber);

        void Update(ProductCatalogue productSerialNumber);

        string GetIdBySerialNumber(int serialNumber);

        int HighestSerialNumber();

        ProductCatalogue GetByKey(int serialNumber);

        ProductCatalogue GetByKey(string serialKey);

        IList<ProductCatalogue> List();

        bool IsReleaseHasProducts(string releaseId);
    }
}