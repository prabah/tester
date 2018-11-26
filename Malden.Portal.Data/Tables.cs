using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Malden.Portal.Data.TableStorage
{
    public static class Tables
    {
        public const string Products = "Products";
        public const string Releases = "Releases";
        public const string Users = "User";
        public const string UserPurchases = "UserPurchases";
        public const string ProductSerialNumbers = "ProductSerialNumbers";
        public const string MaintenanceContracts = "MaintenanceContracts";
        public const string History = "DownloadLogs";
        public const string BlobManager = "BlobManager";
        public const string EmailManager = "EmailManager";
        public const string Distributors = "Distributors";

        public static void CreateTablesIfNotExist()
        {
            var connectionString = ConnectionStringBuilder.GetConnectionString;

            Utilities.Start(connectionString);

            var storageAccount = CloudStorageAccount.FromConfigurationSetting(connectionString);

            var client = storageAccount.CreateCloudTableClient();

            client.CreateTableIfNotExist(Products);
            client.CreateTableIfNotExist(Releases);
            client.CreateTableIfNotExist(Users);
            client.CreateTableIfNotExist(UserPurchases);
            client.CreateTableIfNotExist(ProductSerialNumbers);
            client.CreateTableIfNotExist(MaintenanceContracts);
            client.CreateTableIfNotExist(History);
            client.CreateTableIfNotExist(BlobManager);
            client.CreateTableIfNotExist(EmailManager);
            client.CreateTableIfNotExist(Distributors);
        }
    }


}