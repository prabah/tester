using Malden.Portal.Data.TableStorage.Distributors;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Malden.Portal.Data.TableStorage.History
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly CloudTableClient _client;

        public HistoryRepository()
        {
            var connectionString = ConnectionStringBuilder.GetConnectionString;
            Utilities.Start(connectionString);

            var storageAccount = CloudStorageAccount.FromConfigurationSetting(connectionString);
            _client = storageAccount.CreateCloudTableClient();
        }

        public void Add(IHistory history)
        {
            var context = _client.GetDataServiceContext();
            var historyEntity = new HistoryEntity
                {
                    DateStamp = history.DateStamp,
                    Id = history.Id,
                    PartitionKey = KeyFactory.PartitionKey(history.Id),
                    RowKey = KeyFactory.RowKey(history.UserId),
                    ReleaseId = history.ReleaseId,
                    SerialKeyId = history.SerialKeyId,
                    Timestamp = DateTime.Now,
                    UserId = history.UserId,
                    ImageFileType = history.ImageFileType
                };

            context.AddObject(Tables.History, historyEntity);
            context.SaveChanges();
        }

        public IEnumerable<IHistory> List(int rows = 0)
        {
            return GetDownloads(0, rows);
        }

        public IEnumerable<IHistory> List(string distributorId, int rows = 0)
        {
            return new List<IHistory>();
        }


        public IEnumerable<IHistory> List(int lastRowLoaded, int rows = 0)
        {
            return GetDownloads(lastRowLoaded, rows);
        }

        private IEnumerable<IHistory> GetDownloads(int lastRowLoaded, int rows)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);
           
                if (rows == 0)
                {
                    return (from h in context.CreateQuery<HistoryEntity>(Tables.History)
                            select h).ToList<IHistory>().OrderByDescending(c => c.DateStamp).Skip(lastRowLoaded++).ToList();
                }
                return (from h in context.CreateQuery<HistoryEntity>(Tables.History)
                        select h).ToList<IHistory>().OrderByDescending(c => c.DateStamp).Skip(lastRowLoaded++).ToList().Take(rows);
           
        }

        public IEnumerable<IHistory> ListByUser(string id)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);
            var filteredList = new List<IHistory>();

            var email = (from h in context.CreateQuery<DistributorEntity>(Tables.Distributors)
                         select h).Where(c => c.Id == id).FirstOrDefault().Email;

            return (from h in context.CreateQuery<HistoryEntity>(Tables.History)
                           select h).ToList<IHistory>().Where(c => c.UserId == email).OrderByDescending(c => c.DateStamp).ToList();
        }
        

        public int TotalDownloads()
        {
            return GetDownloads(0, 0).Count();
        }
    }
}