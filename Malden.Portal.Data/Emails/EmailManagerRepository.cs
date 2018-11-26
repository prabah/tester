using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Linq;

namespace Malden.Portal.Data.TableStorage.Emails
{
    public class EmailManagerRepository : IEmailManagerRepository
    {
        private readonly CloudTableClient _client;
        private readonly ITableOperations _tableOpertaions;


        public EmailManagerRepository()
        {
            var connectionString = ConnectionStringBuilder.GetConnectionString;
            Utilities.Start(connectionString);

            var storageAccount = CloudStorageAccount.FromConfigurationSetting(connectionString);

            _client = storageAccount.CreateCloudTableClient();

            _tableOpertaions = new TableOperations(Tables.EmailManager);
        }

        public void Add(IEmailManager emailInstance)
        {
            var softwareEntity = new EmailManagerEntity
            {
                Id = emailInstance.Id,
                UserId = emailInstance.UserId,
                Timestamp = DateTime.UtcNow,
                PartitionKey = KeyFactory.PartitionKey(emailInstance.Id),
                RowKey = KeyFactory.RowKey(emailInstance.UserId),
                ActivatedTime = new DateTime(2010, 1, 1),
                EmailType = emailInstance.EmailType,
                Activated = false
            };

            Delete(emailInstance.UserId, emailInstance.EmailType);

            var context = _client.GetDataServiceContext();
            context.AddObject(Tables.EmailManager, softwareEntity);
            context.SaveChanges();
        }

        public IEmailManager Get(string id, int emailType)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            return (from u in context.CreateQuery<EmailManagerEntity>(Tables.EmailManager)
                    where u.Id == id && u.EmailType == emailType 
                    select u).FirstOrDefault();
        }

        public void Activate(string id, int emailType)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var emailEntity = (from u in context.CreateQuery<EmailManagerEntity>(Tables.EmailManager)
                                 where u.Id == id && u.EmailType == emailType && u.Activated == false 
                                 select u).FirstOrDefault();

            if (emailEntity != null)
            {
                emailEntity.Activated = true;
                emailEntity.PartitionKey = emailEntity.PartitionKey;
                emailEntity.RowKey = emailEntity.RowKey;
                emailEntity.Timestamp = DateTime.UtcNow;
                emailEntity.ActivatedTime = DateTime.UtcNow;
                emailEntity.EmailType = emailType;

                context.UpdateObject(emailEntity);
                context.SaveChanges();
            }
            else
            {
                throw new ArgumentNullException("user email", "The entry cannot be found !");
            }
        }


        public bool IsEmailExpired(string id, int emailType)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var email = (from u in context.CreateQuery<EmailManagerEntity>(Tables.EmailManager)
                    where u.Id == id && u.EmailType == emailType 
                    select u).FirstOrDefault();

            if (email == null) return true;

            return DateTime.UtcNow.AddHours(-24) < email.Timestamp ? false : true;
        }

        public void Delete(string userId, int emailType)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var email = (from u in context.CreateQuery<EmailManagerEntity>(Tables.EmailManager)
                         where u.UserId == userId && u.EmailType == emailType
                         select u).ToList();

            foreach (var em in email)
            {
                context.Detach(em);
                context.AttachTo(Tables.EmailManager, em, "*");
                context.DeleteObject(em);
            }


            context.SaveChanges();
        }

        public bool IsAwaitingActivation(string userId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var email = (from u in context.CreateQuery<EmailManagerEntity>(Tables.EmailManager)
                         where u.Activated == false && u.Id == userId
                         select u).ToList().OrderBy(c => c.Timestamp).LastOrDefault();

            if (email == null) return false;

            var emailStatus = DateTime.UtcNow.AddHours(-24) > email.Timestamp ? false : true;

            return emailStatus;
        }


        public string GetEmailByKey(string key)
        {
            return _tableOpertaions.Get<EmailManagerEntity>(c => c.Id == key).UserId;
        }
    }
}
