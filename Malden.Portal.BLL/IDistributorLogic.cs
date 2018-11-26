using System.Collections.Generic;

namespace Malden.Portal.BLL
{
    public interface IDistributorLogic
    {
        string Add(Distributor distributor);

        Distributor Get(string id);

        Distributor GetByEmail(string email);

        IList<Distributor> List();

        void Delete(Distributor distributor);

        void Update(Distributor distributor);

        void ActivateDistributor(string key, int emailType, string email, string token);

        bool IsDistributorAcccount(string key);

        string EmailByDistributorToken(string token);
    }
}
