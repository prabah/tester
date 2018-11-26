using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Malden.Portal.Data.TableStorage.Releases
{
    public class BlobManagerRepository : IBlobManagerRepository
    {
        private readonly CloudTableClient _client;

        public BlobManagerRepository()
        {
            var connectionString = ConnectionStringBuilder.GetConnectionString;

            Utilities.Start(ConnectionStringBuilder.GetConnectionString);

            var storageAccount = CloudStorageAccount.FromConfigurationSetting(connectionString);

            _client = storageAccount.CreateCloudTableClient();
        }


        public IList<IBlobManager> CountOfBlobFiles(string releaseId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var blobFilesCount = (from u in context.CreateQuery<BlobManagerEntity>(Tables.BlobManager)
                                  where u.ReleaseId == releaseId
                                  select u).ToList<IBlobManager>();

            return blobFilesCount;
        }

        public void UpdateBlobCount(int fileTypeId, string releaseId, int numberOfFiles, string fileName, long fileSize, string fileURL)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var entity = (from u in context.CreateQuery<BlobManagerEntity>(Tables.BlobManager)
                          where (u.FileType == fileTypeId && u.ReleaseId == releaseId)
                          select u).FirstOrDefault();

            entity.NoOfFiles = 1;
            entity.ReleaseId = releaseId;
            entity.FileType = numberOfFiles;
            entity.FileName = fileName;
            entity.FileSize = fileSize;
            entity.FileURL = fileURL;
            entity.PartitionKey = entity.PartitionKey;
            entity.RowKey = entity.RowKey;
            entity.Timestamp = DateTime.Now;


            context.AttachTo(Tables.BlobManager, entity);
            context.SaveChanges();

        }

        public IBlobManager Get(string releaseId, int fileType)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            return (from u in context.CreateQuery<BlobManagerEntity>(Tables.BlobManager)
                    where u.ReleaseId == releaseId && u.FileType == fileType
                    select u).FirstOrDefault();
        }

        public void AddBlobCount(int fileTypeId, string releaseId, int numberOfFiles, string fileName, long fileSize, string fileURL)
        {
            var blobManagerEntity = Get(releaseId, fileTypeId);
            var context = _client.GetDataServiceContext();
            if (blobManagerEntity == null)
            {
                var blobCount = new BlobManagerEntity
                {
                    FileType = fileTypeId,
                    ReleaseId = releaseId,
                    NoOfFiles = numberOfFiles,
                    FileName = fileName,
                    FileSize = fileSize,
                    FileURL = fileURL,
                    PartitionKey = KeyFactory.PartitionKey(releaseId),
                    RowKey = KeyFactory.RowKey(releaseId + fileTypeId.ToString()),
                    Timestamp = DateTime.Now
                };
                context.AddObject(Tables.BlobManager, blobCount);
            }
            else
            {
                blobManagerEntity.NoOfFiles = numberOfFiles;
                blobManagerEntity.FileName = fileName;
                blobManagerEntity.FileSize = fileSize;
                blobManagerEntity.FileURL = fileURL;
                context.AttachTo(Tables.BlobManager, blobManagerEntity);
                context.UpdateObject(blobManagerEntity);
            }
            context.SaveChanges();
        }

        public IList<int> ListOfAvailableBlobs(string releaseId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var blobFiles = (from u in context.CreateQuery<BlobManagerEntity>(Tables.BlobManager)
                             where u.ReleaseId == releaseId && u.NoOfFiles > 0
                             select u).ToList<BlobManagerEntity>();

            return (from file in blobFiles select file.FileType).ToList();
        }

        public string CDNEndPoint()
        {
            return ConnectionStringBuilder.GetCDNEndPoint;
        }
    }
}
