using System.Collections.Generic;

namespace Malden.Portal.BLL
{
    public interface IProductLogic
    {
        void Add(Product product);

        void RemoveById(string id);

        void Update(Product product);

        void Delete(string id);

        IList<Product> List();

        Product GetById(string id);

        Product GetById(int fulfilmentId);

        string ConvertToPortalProductId(int filfilmentProductId);
    }
}