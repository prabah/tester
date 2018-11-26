using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace Malden.Portal.Data.TableStorage.Releases
{
    public class FileManager : IFileManager
    {
        private readonly CloudBlobClient _blobClient;
        private readonly string _connectionString = "";

        public FileManager()
        {
            var connectionString = ConnectionStringBuilder.GetConnectionString;
            _connectionString = connectionString;
            Utilities.Start(ConnectionStringBuilder.GetConnectionString);
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public void Upload(string containerName, string filePath)
        {
            var container = _blobClient.GetContainerReference(containerName);

            container.CreateIfNotExists();

            var fileName = Path.GetFileName(filePath);

            var blob = container.GetBlockBlobReference(fileName);

            blob.Metadata["category"] = "software";
            blob.Metadata["owner"] = "Malden";
            blob.Metadata["key"] = "test_key";
            blob.Metadata["date_added"] = System.DateTime.Now.ToString();
            blob.Properties.CacheControl = "public, max-age=" + Properties.Settings.Default.BlobCacheControlValue;

            using (var fileStream = File.OpenRead(@filePath))
            {
                blob.UploadFromStream(fileStream);
            }
        }

        public Stream Download(string containerName, string releaseFileName)
        {
            var container = _blobClient.GetContainerReference(containerName);

            var file = container.GetBlobReferenceFromServer(releaseFileName);

            var outputFile = new MemoryStream();

            file.DownloadToStream(outputFile);

            outputFile.Seek(0, SeekOrigin.Begin);
            return outputFile;
        }

        public void Upload(string containerName, string releaseFileName, MemoryStream fileStream)
        {
            var container = _blobClient.GetContainerReference(containerName);

            container.CreateIfNotExists();

            //var fileName = System.IO.Path.GetFileName(filePath);

            var blob = container.GetBlockBlobReference(releaseFileName);

            blob.Metadata["category"] = "software";
            blob.Metadata["owner"] = "Malden";
            blob.Metadata["key"] = "test_key";
            blob.Metadata["date_added"] = System.DateTime.Now.ToString();
            blob.Properties.CacheControl = "public, max-age=" + Properties.Settings.Default.BlobCacheControlValue;
            using (fileStream)
            {
                blob.UploadFromStream(fileStream);
            }
        }
    }
}