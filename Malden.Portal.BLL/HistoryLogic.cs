using Malden.Portal.Data;
using System;
using System.Collections.Generic;

namespace Malden.Portal.BLL
{
    public class HistoryLogic : IHistoryLogic
    {
        private readonly IUserLogic _userLogic;
        private readonly IProductCatalogueLogic _serialLogic;
        private readonly IReleaseLogic _relaseLogic;
        private readonly IHistoryRepository _historyRepository;
        private readonly IHistory _history;
        private readonly IProductLogic _productLogic;
        private IUser _user;

        public HistoryLogic(IUserLogic userLogic, IProductCatalogueLogic serialLogic, IReleaseLogic relaseLogic, IHistory history, IHistoryRepository historyRepository, IUser user, IProductLogic productLogic)
        {
            _userLogic = userLogic;
            _serialLogic = serialLogic;
            _relaseLogic = relaseLogic;
            _historyRepository = historyRepository;
            _productLogic = productLogic;
            _history = history;
            _user = user;
        }

        public void Add(History history)
        {
            _history.DateStamp = DateTime.Now;
            _history.Id = Guid.NewGuid().ToString();

            _history.ReleaseId = history.ReleaseDownloaded.Id; //FindReleaseId(history.Version.ToString(), _serialLogic.GetByKey(history.SerialNumber).ProductId);
            _history.SerialKeyId = history.SerialNumber > 0 ? FindSerialKeyId(history.SerialNumber) : ""; 
            _history.ImageFileType = (int)history.ImageFileType;
            _history.UserId = history.UserEmail;


            _historyRepository.Add(_history);
        }

        private string FindSerialKeyId(int serialNumber)
        {
            return _serialLogic.GetIdBySerialNumber(serialNumber);
        }

        private string FindReleaseId(string version, string productId)
        {
            return _relaseLogic.GetByVersion(version, productId).Id;
        }

        public IList<History> List(int rows = 0)
        {
            return ProcessDownloads(_historyRepository.List(rows));


        }

        private IList<History> ProcessDownloads(IEnumerable<IHistory> history)
        {
            var list = new List<History>();

            foreach (var h in history)
            {
                var serailNumber = _serialLogic.GetByKey(h.SerialKeyId);
                var version = _relaseLogic.GetById(h.ReleaseId);

                var user = _userLogic.GetByEmail(h.UserId);

                    list.Add(new History
                    {
                        ImageFileType = (Release.ImageFileType)h.ImageFileType,
                        SerialNumber = serailNumber != null ? serailNumber.SerialNumber : 0,
                        TimeStamp = h.DateStamp,
                        UserEmail = h.UserId,
                        Version = version != null ? version.Version : new Version(0, 0),
                        Product = _productLogic.GetById(version.ProductId).Name,
                        UserType = user != null ? user.TypeOfUser : User.UserType.Customer
                    });
                
            }

            return list;
        }

        public IEnumerable<History> List(int lastRowLoaded, int rows = 0)
        {
            return ProcessDownloads(_historyRepository.List(lastRowLoaded, rows));
        }

        public int TotalDownloads()
        {
            return _historyRepository.TotalDownloads();
        }

        public IEnumerable<History> List(string distributorId)
        {
            return ProcessDownloads(_historyRepository.ListByUser(distributorId));
        }
    }
}