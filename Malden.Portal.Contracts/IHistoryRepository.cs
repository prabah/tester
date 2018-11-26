using System.Collections.Generic;

namespace Malden.Portal.Data
{
    public interface IHistoryRepository
    {
        void Add(IHistory history);

        IEnumerable<IHistory> List(int rows = 0);

        IEnumerable<IHistory> List(int lastRowLoaded, int rows = 0);

        int TotalDownloads();

        IEnumerable<IHistory> ListByUser(string userId);
    }
}