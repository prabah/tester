using System.Collections.Generic;

namespace Malden.Portal.BLL
{
    public interface IUserLogic
    {
        void ActivateUser(string key, int emailType, Malden.Portal.BLL.User.UserType userType = BLL.User.UserType.Customer);

        void Add(User user, Malden.Portal.BLL.User.UserType typeOfUser);

        void Add(User user, string id, Malden.Portal.BLL.User.UserType typeOfUser);

        void BlockUser(string email);

        void DeleteInactiveUsers(int typeOfUser, int cutOffDays = -1);

        User Get(string id);

        User GetByEmail(string email);

        void IncreaseRegisteredProductCounter(string email);

        bool IsEmailInstanceActivated(string activationKey, Malden.Portal.BLL.EmailerLogic.EmailType emailType);

        bool IsEmailInstanceExpired(string activationKey, Malden.Portal.BLL.EmailerLogic.EmailType emailType);

        bool IsValidAdminUser(User signingInUser, string passwordEntered);

        bool IsValidAdminUser(string email);

        bool IsValidUser(User userName, string passwordEntered);

        bool IsValidUser(string userName);

        IList<User> List(Malden.Portal.BLL.User.UserType userType);

        void PasswordReset(string activationKey, string newPassword);

        void Update(User user);

        User User(string emailAddress);
        bool UserAlreadyExixts(string email);

        bool IsAwaitingActivation(string email);

        bool IsDemoDatabase();

        Malden.Portal.BLL.UserLogic.UserLoginStatus LoginStatus(string email, string passwordEntered);
    }
}