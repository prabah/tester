using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System;

namespace Malden.Portal.Data.TableStorage.Releases
{
    public class ReleaseEntity : TableServiceEntity, IRelease
    {
        public string Id { get; set; }

        public string ProductId { get; set; }

        public string Version { get; set; }

        public string ImageFile { get; set; }

        public DateTime DateOfRelease { get; set; }

        public bool IsHidden { get; set; }
    }
}