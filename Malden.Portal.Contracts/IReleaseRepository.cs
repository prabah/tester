using System.Collections.Generic;

namespace Malden.Portal.Data
{
    public interface IReleaseRepository
    {
        void Add(IRelease release);

        void Update(IRelease release);

        void Delete(IRelease release);

        IRelease Get(string id);

        IRelease GetByVersion(string version, string productId);

        IRelease GetByTrimmedId(string trimmedId);

        string ReleaseId(string version, string productId);

        //Refactor
        string LatestRelease(string productId);

        //Refactor
        //string LatestPatchRelease(string productId, string currentReleaseId);
        IList<IRelease> GetReleasesByProductId(string productId);

        IList<IRelease> List();

        bool IsAnyReleasesForProduct(string productId);

        string ReleaseConnectionKey();
    }
}