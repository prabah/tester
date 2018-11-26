using Microsoft.WindowsAzure.Storage.Blob;

namespace Malden.Portal.BLL
{
    public interface IBlobItem
    {
        CloudFile CreateFromIListBlobItem(IListBlobItem item);
    }
}
