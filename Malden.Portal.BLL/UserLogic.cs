using Malden.Portal.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Malden.Portal.BLL
{
    public class UserLogic : IUserLogic
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailManagerRepository _emailRepository;
        private readonly IUser _user;

        public enum UserLoginStatus
        {
            Valid = 0,
            Invalid = 1,
            NotActivated = 2,
            Blocked = 3
        }

        public UserLogic(IUserRepository userRepository, IUser user, IEmailManagerRepository emailRepository)
        {
            _userRepository = userRepository;
            _emailRepository = emailRepository;
            _user = user;
        }


        public bool IsDemoDatabase()
        {
            return _userRepository.IsDemoDatabase();
        }


        public User User(string emailAddress)
        {
            var userData = _userRepository.GetByEmail(emailAddress);

            if (userData == null) return null;

            return new User(userData.Email, userData.Password)
            {
                Company = userData.Company,
                Name = userData.Name,
                TypeOfUser = (BLL.User.UserType) userData.TypeOfUser,
                IsBlocked = userData.IsBlocked,
                Id = userData.Id
            };
        }

        public UserLoginStatus LoginStatus(string email, string passwordEntered)
        {
            var userDetails = User(email);

            if (userDetails == null) return UserLoginStatus.Invalid;

            if (userDetails.IsBlocked && IsAwaitingActivation(userDetails.Id)) return UserLoginStatus.NotActivated;

            if (userDetails.IsBlocked) return UserLoginStatus.Blocked;

            if (Utilities.PasswordResolver.IsValidPass(passwordEntered, userDetails.Password))
                        return  UserLoginStatus.Valid;

            return UserLoginStatus.Invalid;


        }

        public void Add(User user, Malden.Portal.BLL.User.UserType typeOfUser = Malden.Portal.BLL.User.UserType.Customer)
        {
            if (UserAlreadyExixts(user.Email)) throw new DuplicateEntryException("User already exists!");

            _user.Company = user.Company;
            _user.Email = user.Email;
            _user.Name = user.Name;
            _user.Password = Utilities.PasswordResolver.CreateHash(user.Password);
            _user.TypeOfUser = (int) typeOfUser;
            _user.IsBlocked = user.IsBlocked;
            _user.Id = Guid.NewGuid().ToString();

            _userRepository.Add(_user);
        }


        public void Add(User user, string id, Malden.Portal.BLL.User.UserType typeOfUser = Malden.Portal.BLL.User.UserType.Customer)
        {
            if (UserAlreadyExixts(user.Email)) throw new DuplicateEntryException("User already exists!");

            _user.Company = user.Company;
            _user.Email = user.Email;
            _user.Name = user.Name;
            _user.Password = Utilities.PasswordResolver.CreateHash(user.Password);
            _user.TypeOfUser = (int) typeOfUser ;
            _user.IsBlocked = user.IsBlocked;
            _user.Id = id;

            _userRepository.Add(_user);
        }

        public bool IsValidUser(User signingInUser, string passwordEntered)
        {
            var user = User(signingInUser.Email);

            if (user != null)
            {
                if (!user.IsBlocked)
                {
                    return Utilities.PasswordResolver.IsValidPass(passwordEntered, user.Password);
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool IsAwaitingActivation(string userId)
        {
            return _emailRepository.IsAwaitingActivation(userId);
        }

        public bool UserAlreadyExixts(string email)
        {
            return _userRepository.IsUserAlreadyExixts(email);
        }

        public bool IsValidAdminUser(User signingInUser, string passwordEntered)
        {
            var user = User(signingInUser.Email);
            var isValidUser = Utilities.PasswordResolver.IsValidPass(passwordEntered, user.Password);

            return isValidUser && user.TypeOfUser == BLL.User.UserType.Admin;
        }

        public bool IsValidAdminUser(string email)
        {
            if (String.IsNullOrEmpty(email)) return false;
            var user = User(email);
            return user.TypeOfUser == BLL.User.UserType.Admin;
        }

        public void BlockUser(string email)
        {
            _userRepository.Block(_userRepository.GetByEmail(email));
        }

        public IList<User> List(Malden.Portal.BLL.User.UserType userType)
        {
            return _userRepository.List().Where(c => c.TypeOfUser == (int) userType).Select(u => new User(u.Email, u.Password)
            {
                Id = u.Id,
                Name = u.Name,
                Company = u.Company,
                TypeOfUser = (BLL.User.UserType) u.TypeOfUser,
                IsBlocked = u.IsBlocked
            }).ToList();
        }

        public bool IsValidUser(string userName)
        {
            var userData = _userRepository.GetByEmail(userName);

            return (userData != null);
        }

        public User Get(string id)
        {
            var userData = _userRepository.Get(id);

            if (userData == null) return null;

            return new User(userData.Email, userData.Password)
            {
                Email = userData.Email,
                Company = userData.Company,
                Name = userData.Name,
                TypeOfUser = (BLL.User.UserType) userData.TypeOfUser,
                IsBlocked = userData.IsBlocked,
                Id = userData.Id
            };
        }

        public User GetByEmail(string email)
        {
            var userData = _userRepository.GetByEmail(email);

            if (userData == null) return null;

            return new User(userData.Email, userData.Password)
            {
                Email = userData.Email,
                Company = userData.Company,
                Name = userData.Name,
                TypeOfUser = (BLL.User.UserType)userData.TypeOfUser,
                IsBlocked = userData.IsBlocked,
                Id = userData.Id
            };
        }

        public void Update(User user)
        {
            if (!UserAlreadyExixts(user.Email)) throw new NotFoundException("No user account details!");

            _user.Company = user.Company;
            _user.Name = user.Name;
            _user.TypeOfUser = (int) user.TypeOfUser;
            _user.IsBlocked = user.IsBlocked;
            _user.Id = user.Id;

            _userRepository.Update(_user);
        }

        public bool IsEmailInstanceActivated(string activationKey, Malden.Portal.BLL.EmailerLogic.EmailType emailType)
        {
            var emailInstance = _emailRepository.Get(activationKey, (int)emailType);
            return emailInstance != null ? emailInstance.Activated : false;
        }

        public bool IsEmailInstanceExpired(string activationKey, Malden.Portal.BLL.EmailerLogic.EmailType emailType)
        {
            return _emailRepository.IsEmailExpired(activationKey, (int)emailType);
        }

        public void PasswordReset(string activationKey, string newPassword)
        {
            var emailType = (int)Malden.Portal.BLL.EmailerLogic.EmailType.ResetPassword;

            if (_emailRepository.IsEmailExpired(activationKey, emailType)) throw new InvalidOperationException("Email link has expired!");
            var emailInstance = _emailRepository.Get(activationKey, emailType);
            if (emailInstance.Activated) throw new InvalidOperationException("Password already changed");
            var user = Get(emailInstance.UserId);
            if (user.IsBlocked) throw new InvalidOperationException("Please activate your account!");

            _user.Company = user.Company;
            _user.Company = user.Company;
            _user.Name = user.Name;
            _user.TypeOfUser = (int) user.TypeOfUser;
            _user.IsBlocked = user.IsBlocked;
            _user.Password = Utilities.PasswordResolver.CreateHash(newPassword);
            _user.Id = user.Id;

            _userRepository.Update(_user, true);
            UpdateEmailInstance(activationKey, emailType);
        }

        public void IncreaseRegisteredProductCounter(string email)
        {
            _userRepository.IncreaseRegisteredProductCounter(email);
        }

        public void DeleteInactiveUsers(int typeOfUser, int cutOffDays = -1)
        {
            _userRepository.DeleteInactiveUsers(typeOfUser, cutOffDays);
        }


        public void ActivateUser(string key, int emailType, Malden.Portal.BLL.User.UserType userType = BLL.User.UserType.Customer)
        {
            if (_emailRepository.IsEmailExpired(key, emailType)) throw new InvalidOperationException("Email link has expired!");

            var emailInstance = _emailRepository.Get(key, emailType);

            if (emailInstance.Activated) throw new InvalidOperationException("Email is already activated");

            var user = Get(emailInstance.UserId);
            user.IsBlocked = false;
            user.TypeOfUser = userType;

            UpdateEmailInstance(key, emailType);

            Update(user);

        }

        private void UpdateEmailInstance(string key, int emailType)
        {
            _emailRepository.Activate(key, emailType);
        }
    }
}