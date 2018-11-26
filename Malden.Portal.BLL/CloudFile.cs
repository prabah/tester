using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace Malden.Portal.BLL
{
    public class CloudFile
    {
        public string FileName { get; set; }

        public string URL { get; set; }

        public long Size { get; set; }

        public long BlockCount { get; set; }

        public CloudBlockBlob BlockBlob { get; set; }

        public DateTime StartTime { get; set; }

        public string UploadStatusMessage { get; set; }

        public bool IsUploadCompleted { get; set; }

        public string FileKey { get; set; }

        public int FileIndex { get; set; }

        public Malden.Portal.BLL.Release.ImageFileType ImageFileType { get; set; }

        public string ReleaseId { get; set; }

        public string ContainerName { get; set; }

        public static CloudFile CreateFromIListBlobItem(IListBlobItem item, string cdnEndpoint)
        {
            if (item is CloudBlockBlob)
            {
                var blob = (CloudBlockBlob)item;
                blob.FetchAttributes();

                var urlPathAndQuery = blob.Uri.PathAndQuery;

                return new CloudFile
                {
                    FileName = blob.Name,
                    URL = cdnEndpoint + urlPathAndQuery,
                    Size = blob.Properties.Length,
                    ReleaseId = blob.Metadata["ReleaseId"] != null ? blob.Metadata["ReleaseId"].ToString() : "",
                    ContainerName = blob.Container.Name
                };
            }
            return null;
        }
    }
}