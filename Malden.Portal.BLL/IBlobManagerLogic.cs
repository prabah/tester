using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;

namespace Malden.Portal.BLL
{
    public interface IBlobManagerLogic
    {
        void RefreshBlobFileCount(string releaseId);
        IList<BlobManager> CountOfBlobFiles(string releaseId);
        void AddBlobCount(int fileTypeId, string releaseId, int numberOfFiles, string fileName, long fileSize, string fileURL);
        void UpdateBlobCount(int fileTypeId, string releaseId, int numberOfFiles, string fileName, long fileSize, string fileURL);
        CloudBlobContainer GetContainer(string containerName);
        IList<int> ListOfAvailableBlobs(string releaseId);
        IList<CloudFile> CloudFiles(string releaseId);
        string CDNEndPoint();
    }
}
