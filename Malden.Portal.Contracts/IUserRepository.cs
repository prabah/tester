using System.Collections.Generic;

namespace Malden.Portal.Data
{
    public interface IUserRepository
    {
        void Add(IUser newUser);

        void Update(IUser user, bool passwordReset = false);

        void Block(IUser user);

        void Delete(string email);

        IUser Get(string id);

        IUser GetByEmail(string email);

        bool IsUserAlreadyExixts(string email);

        IList<IUser> List();

        void DeleteInactiveUsers(int typeOfUser, int cutOffDays);

        void IncreaseRegisteredProductCounter(string email);

        void ActivateUser(string id);

        bool IsDemoDatabase();

    }
}