using System;

namespace Malden.Portal.Data
{
    public interface IDownloadHistory
    {
        string Id { get; set; }

        string UserId { get; set; }

        string SerialKeyId { get; set; }

        string ReleaseId { get; set; }

        DateTime DateStamp { get; set; }
    }
}