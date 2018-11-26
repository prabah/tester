using System.Collections.Generic;

namespace Malden.Portal.Data
{
    public interface IBlobManagerRepository
    {
        IList<IBlobManager> CountOfBlobFiles(string releaseId);

        void UpdateBlobCount(int fileTypeId, string releaseId, int numberOfFiles, string fileName, long fileSize, string fileURL);

        void AddBlobCount(int fileTypeId, string releaseId, int numberOfFiles, string fileName, long fileSize, string fileURL);

        IList<int> ListOfAvailableBlobs(string releaseId);

        string CDNEndPoint();
    }
}
