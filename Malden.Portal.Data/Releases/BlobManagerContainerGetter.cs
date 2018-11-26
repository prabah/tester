using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Malden.Portal.Data.TableStorage.Releases
{
    public class BlobManagerContainerGetter : IBlobManagerContainerGetter
    {
        private readonly string _connectionString;

        public BlobManagerContainerGetter()
        {
            _connectionString = ConnectionStringBuilder.GetConnectionString;
        }

        public CloudBlobContainer GetContainer(string containerName)
        {
            var container = CloudStorageAccount.Parse(_connectionString).CreateCloudBlobClient() 
                .GetContainerReference(containerName); 

            container.CreateIfNotExists();
            return container;
        }

        public CloudBlobClient GetStorageClient()
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var storageClient = storageAccount.CreateCloudBlobClient();
            return storageClient;
        }
    }
}
