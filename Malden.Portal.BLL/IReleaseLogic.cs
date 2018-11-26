using System.Collections.Generic;
using System.IO;

namespace Malden.Portal.BLL
{
    public interface IReleaseLogic
    {
        void Add(Release release);

        void Update(Release release);

        void Delete(string id);

        void Upload(MemoryStream fileStream, string productId, string releaseFileName);

        Release GetByVersion(string version, string productId);

        Release GetById(string id);

        Release LatestRelease(int serialNumber, string productId, bool isProductMaintained);

        Release LatestReleaseByDate(int serialNumber, string productId, bool isProductMaintained);

        IList<Release> ReleasesByTheProduct(Release currentRelease, bool newer);

        IList<Release> List();

        IList<Release> OldReleases(int serialNumber);

        IList<Release> OldReleasesByDate(int serialNumber);

        List<Release> GetMajorReleases(IList<Release> oldReleases);

        Stream Download(string userId, int serial);

        Stream Download(string userId, string trimmedCurrentReleaseId, int serial);

        //TODO:::: REMOVE
        string ReleaseConnectionKey();

        string ContainerName(Malden.Portal.BLL.Release.ImageFileType imageType, string releaseId);

        Release GetReleasesByProductId(string productId);

        IList<Release> Archive(string productId);
    }
}