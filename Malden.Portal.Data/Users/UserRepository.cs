using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Malden.Portal.Data.TableStorage.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly CloudTableClient _client;

        public UserRepository()
        {
            var connectionString = ConnectionStringBuilder.GetConnectionString;

            Utilities.Start(connectionString);

            var storageAccount = CloudStorageAccount.FromConfigurationSetting(connectionString);

            _client = storageAccount.CreateCloudTableClient();

            _client.CreateTableIfNotExist(Tables.Users);
        }



        public void Add(IUser newUser)
        {
            var mailAddress = new MailAddress(newUser.Email);

            var userEntity = new UserEntity
            {
                Company = newUser.Company,
                Email = newUser.Email,
                Name = newUser.Name,
                Password = newUser.Password,
                RegisteredProductsCount = 0,
                PartitionKey = KeyFactory.PartitionKey(mailAddress.Host),
                RowKey = KeyFactory.RowKey(mailAddress.User),
                Timestamp = DateTime.Now,
                IsBlocked = newUser.IsBlocked,
                TypeOfUser = newUser.TypeOfUser,
                Id = newUser.Id
            };

            var context = _client.GetDataServiceContext();

            context.AddObject(Tables.Users, userEntity);
            context.SaveChanges();
        }

        public IUser GetByEmail(string emailAddress)
        {
            try
            {
                if (emailAddress.Length > 0)
                {
                    var mailAddress = new MailAddress(emailAddress);

                    var context = _client.GetDataServiceContext();
                    Utilities.SetNotFoundException(context);

                    var user = (from u in context.CreateQuery<UserEntity>(Tables.Users)
                                where u.RowKey == KeyFactory.RowKey(mailAddress.User) && u.PartitionKey == KeyFactory.PartitionKey(mailAddress.Host)
                                select u).FirstOrDefault();
                    return user;
                }
                else return null;
            }
            catch (Exception ex)
            {
                if (ex.GetType() ==typeof(FormatException))
                {

                    return null;
                }
                else 
                throw new Exception("Errors occurred while logging in");
            }
            finally
            {

            }
        }

        public bool IsUserAlreadyExixts(string email)
        {
            //new MailAddress(email);

            var user = GetByEmail(email);

            return user != null;
        }

        public void Block(IUser user)
        {
            if (user != null)
            {
                user.IsBlocked = true;
                var context = _client.GetDataServiceContext();
                context.AttachTo(Tables.Users, user);
                context.UpdateObject(user);
                context.SaveChanges();
            }
        }

        public IList<IUser> List()
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            return (from u in context.CreateQuery<UserEntity>(Tables.Users)
                    select u).ToList<IUser>();
        }

        public IUser Get(string id)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            return (from u in context.CreateQuery<UserEntity>(Tables.Users)
                    where u.Id == id
                    select u).FirstOrDefault();
        }

        public void Update(IUser user, bool passwordReset = false)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var userEntity = (from u in context.CreateQuery<UserEntity>(Tables.Users)
                              where u.Id == user.Id
                              select u).FirstOrDefault();

            if (userEntity != null)
            {
                userEntity.Company = user.Company;
                userEntity.Email = userEntity.Email;
                userEntity.Name = user.Name;
                userEntity.Password = passwordReset ? user.Password : userEntity.Password;
                userEntity.PartitionKey = userEntity.PartitionKey;
                userEntity.RowKey = userEntity.RowKey;
                userEntity.Timestamp = DateTime.Now;
                userEntity.IsBlocked = user.IsBlocked;
                userEntity.RegisteredProductsCount = userEntity.RegisteredProductsCount;
                userEntity.TypeOfUser = user.TypeOfUser;
                userEntity.Id = user.Id;
                context.UpdateObject(userEntity);
                context.SaveChanges();
            }
        }

        public void IncreaseRegisteredProductCounter(string email)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var userEntity = (from u in context.CreateQuery<UserEntity>(Tables.Users)
                              where u.Email == email
                              select u).FirstOrDefault();

            if (userEntity != null)
            {
                userEntity.RegisteredProductsCount = userEntity.RegisteredProductsCount + 1;
                context.UpdateObject(userEntity);
                context.SaveChanges();
            }
        }

        public void DeleteInactiveUsers(int typeOfUser, int cutOffDays)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var cutOffTime = System.DateTime.UtcNow.AddDays(cutOffDays);

            var query = (from u in context.CreateQuery<UserEntity>(Tables.Users)
                         where u.RegisteredProductsCount == 0 && u.Timestamp <= cutOffTime && u.TypeOfUser == typeOfUser
                         select u).ToList();

            
            foreach (var inactiveUser in query)
            {
                context.Detach(inactiveUser);
                context.AttachTo(Tables.Users, inactiveUser, "*");
                context.DeleteObject(inactiveUser);

            }
            context.SaveChanges();
        }

        public void Delete(string email)
        {
            var user = (UserEntity)GetByEmail(email);

            if (user != null)
            {
                var context = _client.GetDataServiceContext();
                context.AttachTo(Tables.Users, user, "*");
                context.DeleteObject(user);
                context.SaveChanges();
            }
        }

        public void ActivateUser(string id)
        {
            var user = Get(id);
            user.IsBlocked = false;
            Update(user);
        }


        public bool IsDemoDatabase()
        {
            return ConnectionStringBuilder.GetConnectionString.IndexOf(Properties.Settings.Default.LiveDbName) < 0;

        }
    }
}