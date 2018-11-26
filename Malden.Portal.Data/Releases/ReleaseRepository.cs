using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Malden.Portal.Data.TableStorage.Releases
{
    public class ReleaseRepository : IReleaseRepository
    {
        private readonly CloudTableClient _client;

        public ReleaseRepository()
        {
            var connectionString = ConnectionStringBuilder.GetConnectionString;

            Utilities.Start(ConnectionStringBuilder.GetConnectionString);

            var storageAccount = CloudStorageAccount.FromConfigurationSetting(connectionString);

            _client = storageAccount.CreateCloudTableClient();
        }

        public void Add(IRelease release)
        {
            var releaseEntity = new ReleaseEntity
            {
                Id = release.Id,
                PartitionKey = KeyFactory.PartitionKey(release.Id),
                RowKey = KeyFactory.RowKey(release.ProductId),
                Timestamp = DateTime.Now,
                Version = release.Version,
                ProductId = release.ProductId,
                ImageFile = release.ImageFile,
                DateOfRelease = release.DateOfRelease,
                IsHidden = release.IsHidden
            };

            var context = _client.GetDataServiceContext();
            context.AddObject(Tables.Releases, releaseEntity);
            context.SaveChanges();
        }

        public IRelease Get(string id)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            return (from u in context.CreateQuery<ReleaseEntity>(Tables.Releases)
                    where u.Id == id
                    select u).FirstOrDefault();
        }

        public string ReleaseId(string versionString, string productId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var release = (context.CreateQuery<ReleaseEntity>(Tables.Releases).Where(u => u.Version == versionString)).FirstOrDefault();

            return release != null ? release.Id : "";
        }

        public IRelease GetByVersion(string versionString, string productId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var releaseEntity = (from u in context.CreateQuery<ReleaseEntity>(Tables.Releases)
                                 where u.Version == versionString &&
                                        u.ProductId == productId
                                 select u).FirstOrDefault();

            return releaseEntity;
        }

        public string LatestRelease(string productId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var lastOrDefault = (from u in context.CreateQuery<ReleaseEntity>(Tables.Releases)
                                 where u.ProductId == productId && u.IsHidden == false
                                 select u).ToList().OrderBy(c => c.Version).LastOrDefault();

            return lastOrDefault != null ? lastOrDefault.Version : null;
        }

        public IList<IRelease> GetReleasesByProductId(string productId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var releases = (from u in context.CreateQuery<ReleaseEntity>(Tables.Releases)
                            where u.ProductId == productId
                            select u).ToList<IRelease>();

            return releases;
        }


        public IList<IRelease> List()
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var releases = (from u in context.CreateQuery<ReleaseEntity>(Tables.Releases)
                            select u).ToList<IRelease>();

            return releases;
        }

        public void Update(IRelease release)
        {
            var releaseEntity = (ReleaseEntity)Get(release.Id);

            if (releaseEntity != release)
            {
                releaseEntity.Version = release.Version;
                releaseEntity.DateOfRelease = release.DateOfRelease;
                releaseEntity.PartitionKey = releaseEntity.PartitionKey;
                releaseEntity.RowKey = releaseEntity.RowKey;
                releaseEntity.Timestamp = DateTime.Now;
                releaseEntity.ImageFile = release.ImageFile;
                releaseEntity.IsHidden = release.IsHidden;

                var context = _client.GetDataServiceContext();
                context.AttachTo(Tables.Releases, releaseEntity);
                context.UpdateObject(releaseEntity);
                context.SaveChanges();
            }
        }

        public void Delete(IRelease release)
        {
            var releaseEntity = (ReleaseEntity)Get(release.Id);

            if (release != null)
            {
                var context = _client.GetDataServiceContext();
                context.AttachTo(Tables.Releases, releaseEntity, "*");
                context.DeleteObject(releaseEntity);
                context.SaveChanges();
            }
        }

        public bool IsAnyReleasesForProduct(string productId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var releaseCount = (from u in context.CreateQuery<ReleaseEntity>(Tables.Releases)
                                where u.ProductId == productId
                                select u).ToList().Count();

            return (releaseCount > 0);
        }

        public IRelease GetByTrimmedId(string trimmedId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var releases = (from u in context.CreateQuery<ReleaseEntity>(Tables.Releases) select u).ToList();

            var release = releases.Where(r => r.Id.Substring(0, 8).Equals(trimmedId)).FirstOrDefault();

            return release;
        }

        public string ReleaseConnectionKey()
        {
            var connectionString = ConnectionStringBuilder.GetConnectionString;
            return ConnectionStringBuilder.GetConnectionString;
        }
    }
}