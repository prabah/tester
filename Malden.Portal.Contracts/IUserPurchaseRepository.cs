using System.Collections.Generic;

namespace Malden.Portal.Data
{
    public interface IUserPurchaseRepository
    {
        void Add(IUserPurchase userPurchase, string id);

        void Remove(string id);

        void Update(IUserPurchase userPurchase, string email);

        IUserPurchase GetBySerialNumber(int serialNumber, string email);

        IList<IUserPurchase> UserPurchasesByEmail(string email);

        IUserPurchase Get(string id);

        bool IsValidSerialNumber(string userId, int serialNumber);
    }
}