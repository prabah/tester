using Microsoft.WindowsAzure.Storage.Blob;

namespace Malden.Portal.BLL
{
    public class BlobItemBlobs : IBlobItem
    {
        public CloudFile CreateFromIListBlobItem(IListBlobItem item)
        {
            if (item is CloudBlockBlob)
            {
                var blob = (CloudBlockBlob)item;
                blob.FetchAttributes();

                return new CloudFile
                {
                    FileName = blob.Name,
                    URL = blob.Uri.ToString(),
                    Size = blob.Properties.Length,
                    ReleaseId = blob.Metadata["ReleaseId"] != null ? blob.Metadata["ReleaseId"].ToString() : "",
                    ContainerName = blob.Container.Name
                };
            }
            return null;
        }
    }
}
