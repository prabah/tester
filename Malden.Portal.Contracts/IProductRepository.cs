using System.Collections.Generic;

namespace Malden.Portal.Data
{
    public interface IProductRepository
    {
        void Add(IProduct product);

        void Delete(string id);

        void Update(IProduct product);

        void Delete(IProduct product);

        IList<IProduct> List();

        IProduct GetById(string id);

        IProduct GetByFulfilmentId(int fulfilmentId);

        string ConvertToPortalProductId(int fulfilmentProductId);

        bool IsAnyReleasesForProduct(string productId);
    }
}