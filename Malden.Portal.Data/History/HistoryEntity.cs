using Microsoft.WindowsAzure.StorageClient;
using System;

namespace Malden.Portal.Data.TableStorage.History
{
    public class HistoryEntity : TableServiceEntity, IHistory
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string SerialKeyId { get; set; }

        public string ReleaseId { get; set; }

        public DateTime DateStamp { get; set; }

        public int ImageFileType { get; set; }
    }
}