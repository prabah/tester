using System.Collections.Generic;

namespace Malden.Portal.BLL
{
    public interface IUserPurchaseLogic
    {
        void Add(UserPurchase userPurchase, string email);

        void Update(UserPurchase userPurchase, string email);

        IList<UserPurchase> List(string email);

        UserPurchase GetById(string id);

        bool IsValidPurchase(string userName, int serialNumber);

        bool IsValidSerialNumber(string email, string registrationCode, bool fullValidation = true);

        IList<Release> LatestReleasesForAllProducts();
    }
}