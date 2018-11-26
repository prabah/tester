
using Microsoft.WindowsAzure.StorageClient;
namespace Malden.Portal.Data.TableStorage.Releases
{
    public class BlobManagerEntity : TableServiceEntity, IBlobManager 
    {
        public string ReleaseId { get; set; }
        public int FileType { get; set; }
        public int NoOfFiles { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string FileURL { get; set; }
    }
}
