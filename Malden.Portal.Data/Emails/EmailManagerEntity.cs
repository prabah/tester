using Microsoft.WindowsAzure.StorageClient;
using System;

namespace Malden.Portal.Data.TableStorage.Emails
{
    public class EmailManagerEntity : TableServiceEntity, IEmailManager
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime ActivatedTime { get; set; }
        public bool Activated { get; set; }
        public int EmailType { get; set; }
    }
}
