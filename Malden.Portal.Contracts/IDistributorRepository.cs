
using System.Collections.Generic;
namespace Malden.Portal.Data
{
    public interface IDistributorRepository
    {
        void Add(IDistributor distributor);

        IDistributor Get(string id);

        IDistributor GetByEmail(string email);

        IList<IDistributor> List();

        void Delete(IDistributor distributor);

        void Update(IDistributor distributor);

        IDistributor GetByToken(string token);
    }
}
