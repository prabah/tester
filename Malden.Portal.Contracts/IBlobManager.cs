
namespace Malden.Portal.Data
{
    public interface IBlobManager
    {
        string ReleaseId { get; set; }
        int FileType { get; set; }
        int NoOfFiles { get; set; }
        string FileName { get; set; }
        long FileSize { get; set; }
        string FileURL { get; set; }
    }
}
