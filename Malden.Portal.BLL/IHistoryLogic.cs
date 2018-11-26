using System.Collections.Generic;

namespace Malden.Portal.BLL
{
    public interface IHistoryLogic
    {
        void Add(History history);

        IList<History> List(int rows = 0);

        IEnumerable<History> List(int lastRowLoaded, int rows = 0);

        int TotalDownloads();

        IEnumerable<History> List(string distributorId);
    }
}