using Microsoft.WindowsAzure.Storage.Blob;

namespace Malden.Portal.Data
{
    public interface IBlobManagerContainerGetter
    {
        CloudBlobContainer GetContainer(string containerName);

        CloudBlobClient GetStorageClient();
    }
}
