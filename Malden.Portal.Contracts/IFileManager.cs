using System.IO;

namespace Malden.Portal.Data
{
    public interface IFileManager
    {
        void Upload(string containerName, string filePath);

        void Upload(string containerName, string releaseFileName, MemoryStream fileStream);

        Stream Download(string containerName, string releaseFileName);
    }
}