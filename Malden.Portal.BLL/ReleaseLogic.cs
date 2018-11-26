using Malden.Portal.BLL.Utilities;
using Malden.Portal.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Malden.Portal.BLL
{
    public class ReleaseLogic : IReleaseLogic
    {
        private readonly IReleaseRepository _releaseRepository;
        private readonly IRelease _release;
        private readonly IMaintenanceContractRepository _maintenanceContractRepository;
        private readonly IProductCatalogueRepository _serialNumberRepository;
        private readonly IProductRepository _productRepository;
        private readonly IFileManager _fileManagerRepository;
        private readonly IHistoryRepository _historyRepository;
        private readonly IHistory _history;

        public ReleaseLogic
        (
        IReleaseRepository releaseRepository,
        IRelease release,
        IMaintenanceContractRepository maintenanceContractRepository,
        IProductCatalogueRepository serialNumberRepository,
        IProductRepository productRepository,
        IFileManager fileManagerRepository,
        IHistoryRepository historyRepository,
        IHistory history
        )
        {
            _release = release;
            _releaseRepository = releaseRepository;
            _maintenanceContractRepository = maintenanceContractRepository;
            _serialNumberRepository = serialNumberRepository;
            _productRepository = productRepository;
            _fileManagerRepository = fileManagerRepository;
            _historyRepository = historyRepository;
            _history = history;
        }

        public void Upload(MemoryStream fileStream, string productId, string releaseFileName)
        {
            var containerName = _productRepository.GetById(productId).ContainerName;

            _fileManagerRepository.Upload(containerName, releaseFileName, fileStream);
        }

        public void Add(Release release)
        {
            var isRecordExist = GetByVersion(release.Version.ToString(), release.ProductId);

            if (isRecordExist == null)
            {
                _release.Id = Guid.NewGuid().ToString();
                _release.Version = release.Version.ToString();
                _release.ProductId = release.ProductId;
                _release.DateOfRelease = release.DateOfRelease;
                _release.IsHidden = release.IsHidden;

                _releaseRepository.Add(_release);
            }
            else
            {
                throw new DuplicateEntryException("Release is already exists for the selected product");
            }
        }

        public Release GetByVersion(string version, string productId)
        {
            var releaseEntity = _releaseRepository.GetByVersion(version, productId);

            if (releaseEntity == null) return null;
            
            return ProcessRelease(releaseEntity);
        }

        private bool IsMaintenanceContractAvailable(string serialKeyId)
        {
            return (_maintenanceContractRepository.GetDateOfExpiry(serialKeyId) >= DateTime.Now);
        }

        public IList<Release> ReleasesByProduct(Release currentRelease)
        {
            //var versions = new Dictionary<string, Version>();

            var productReleases = (from release in _releaseRepository.GetReleasesByProductId(currentRelease.ProductId)
                                   select release).ToList()
                                   .Select(r => ProcessRelease(r)).ToList();

            return productReleases.OrderBy(r => r.Version).ToList();
        }

        public IList<Release> Archive(string productId)
        {
            var productReleases = (from release in _releaseRepository.GetReleasesByProductId(productId)
                                   where (release.IsHidden == false)
                                   select release).ToList().Select(r => ProcessRelease(r)).ToList();


            var filteredReleases = GetMajorReleases(productReleases);

            return filteredReleases;
        }



        private IList<Release> MaxReleasesByProduct(Release currentRelease, bool isProductMaintained, DateTime dateOfMaintenanceExpiry, bool isMaintenanceContractAvailable)
        {
            var filteredReleases = Archive(currentRelease.ProductId);

            //var productReleases = (from r in _releaseRepository.GetReleasesByProductId(currentRelease.ProductId)
            //                       where (r.IsHidden == false)
            //                       select r).ToList().Select(r => new Release(r.Version)
            //                       {
            //                           DateOfRelease = r.DateOfRelease,
            //                           Id = r.Id,
            //                           ProductId = r.ProductId,
            //                           IsHidden = r.IsHidden,
            //                           Version = new Version(r.Version)
            //                       }).ToList();

            //filteredReleases = GetMajorReleases(productReleases);

            if (!isProductMaintained)
            {
                filteredReleases = (from r in filteredReleases
                                    select r).ToList();
            }
            else
            {
                if (isMaintenanceContractAvailable)
                filteredReleases = (from r in filteredReleases
                                    where (r.DateOfRelease <= dateOfMaintenanceExpiry)
                                    select r).ToList();
                else
                    filteredReleases = (from r in filteredReleases
                                        where (r.Version.Major <= currentRelease.Version.Major && r.Version.Minor <= currentRelease.Version.Minor)
                                        select r).ToList();
            }

            return filteredReleases;
        }

        private IList<Release> ReleasesWithoutBuildVersions(IList<Release> oldReleases)
        {
            var downloadableReleases = new List<Release>();

            var majorReleases = (from r in oldReleases group r by new { r.Version.Major }).ToList();

            foreach (var key in majorReleases)
            {
                var majorMinorFilteredList = (from or in oldReleases
                                              where or.Version.Major == key.FirstOrDefault().Version.Major
                                              select or).ToList();

                var release = (from rel in majorMinorFilteredList
                               where rel.Version.Minor == majorMinorFilteredList.Max(m => m.Version.Minor)
                               select rel).FirstOrDefault();

                downloadableReleases.Add(release);
            }

            return downloadableReleases;
        }

        private IList<Release> ReleasesWithBuildVersions(IList<Release> oldReleases)
        {
            var downloadableReleases = new List<Release>();

            var majorReleases = (from r in oldReleases
                                 where
                                 r.Version.Build == oldReleases.Where(m => m.Version.Major == r.Version.Major && m.Version.Minor == r.Version.Minor).Max(c => c.Version.Build)
                                 group r by new { r.Version.Major, r.Version.Minor }

                                 ).ToList();

            foreach (var key in majorReleases)
            {
                foreach (Release r in key)
                {
                    var majorMinorFilteredList = (from oldRelease in oldReleases
                                                  where oldRelease.Version.Major == r.Version.Major && oldRelease.Version.Minor == r.Version.Minor
                                                  select oldRelease).ToList();

                    var release = (from rel in majorMinorFilteredList
                                   where rel.Version.Build == majorMinorFilteredList.Max(m => m.Version.Build)
                                   select rel).FirstOrDefault();

                    downloadableReleases.Add(release);
                }
            }

            return downloadableReleases;
        }

        public List<Release> GetMajorReleases(IList<Release> oldReleases)
        {
            var buildVersionCount = (from r in oldReleases
                                     where r.Version.Build > 0
                                     select r).Count();

            //var downloadableReleases = new List<Release>();

            if (buildVersionCount > 0)
            {
                return ReleasesWithBuildVersions(oldReleases).ToList<Release>();
            }
            else
            {
                return ReleasesWithoutBuildVersions(oldReleases).ToList<Release>();
            }
        }

        //TODO: Remove
        private IList<Release> MaxReleasesByTheProduct(Release currentRelease, bool isProductMaintained, bool isMaintenanceContractAvailable)
        {
            //var versions = new Dictionary<string, Version>();
            IList<Release> filteredReleases;
            //var isMaintained = _productRepository.GetById(currentRelease.ProductId).IsMaintained;

            var productReleases = (from release in _releaseRepository.GetReleasesByProductId(currentRelease.ProductId)
                                   where (release.Id != currentRelease.Id && release.IsHidden == false)
                                   select release).ToList().Select(r => ProcessRelease(r)).ToList();

            //foreach (var r in productReleases)
            //{
            //    versions.Add(r.Id, r.Version);
            //}

            if (!isProductMaintained || isMaintenanceContractAvailable)
            {
                filteredReleases = (from r in productReleases
                                    select r).ToList();
            }
            else
            {
                filteredReleases = (from r in productReleases
                                    where (r.Version < currentRelease.Version)
                                    select r).ToList();
            }

            return filteredReleases;
        }

        public IList<Release> ReleasesByTheProduct(Release currentRelease, bool newer)
        {
            //var versions = new Dictionary<string, Version>();
            IList<Release> filteredReleases;

            var productReleases = (from release in _releaseRepository.GetReleasesByProductId(currentRelease.ProductId)
                                   where (release.Id != currentRelease.Id)
                                   select release).ToList().Select(r => ProcessRelease(r)).ToList();


            if (newer)
            {
                filteredReleases = (from r in productReleases
                                    where r.Version > currentRelease.Version
                                    select r).OrderBy(r => r.Version).ToList();
            }
            else
            {
                filteredReleases = (from r in productReleases
                                    where (r.Version < currentRelease.Version)
                                    select r).OrderBy(r => r.Version).ToList();
            }

            return filteredReleases;
        }

        public Release LatestRelease(int serialNumber, string productId, bool isProductMaintained)
        {
            var purchaseDetails = _serialNumberRepository.GetProductDetailsBySerialNumber(serialNumber);

            if (purchaseDetails == null) return null;
            var currentRelease = CurrentRelease(purchaseDetails);
            var isMaintenanaceContractAvailable = IsMaintenanceContractAvailable(purchaseDetails.Id);
            var filteredReleases = ReleasesByProduct(currentRelease);

            if (!isProductMaintained)
            {
                return filteredReleases != null ? filteredReleases.LastOrDefault() : null;
            }

            //Change the logic here on date of release
            if (isMaintenanaceContractAvailable)
            {
                return filteredReleases != null ? filteredReleases.LastOrDefault() : null;
            }
            else
            {
                var latestReleases = filteredReleases.Where(r =>
                    r.Version.Major == currentRelease.Version.Major &&
                    r.Version.Minor == currentRelease.Version.Minor &&
                    r.Version.Build >= currentRelease.Version.Build &&
                    r.Version.Revision > currentRelease.Version.Revision);

                return latestReleases != null ? latestReleases.LastOrDefault() : null;
            }
        }

        public Release LatestReleaseByDate(int serialNumber, string productId, bool isProductMaintained)
        {
            var purchaseDetails = _serialNumberRepository.GetProductDetailsBySerialNumber(serialNumber);

            if (purchaseDetails == null) return null;

            var currentRelease = CurrentRelease(purchaseDetails);
            var isMaintenanaceContractAvailable = IsMaintenanceContractAvailable(purchaseDetails.Id);
            var filteredReleases = ReleasesByProduct(currentRelease);

            if (!isProductMaintained)
            {
                return filteredReleases != null ? filteredReleases.LastOrDefault() : null;
            }

            var dateOfMaintenanceExpiry = _maintenanceContractRepository.GetDateOfExpiry(purchaseDetails.Id);

            //Change the logic here on date of release


            if (isMaintenanaceContractAvailable)
            {
                return filteredReleases != null ? filteredReleases.Where(r => r.IsHidden == false).LastOrDefault() : null;
            }
            else
            {
                var latestReleases = filteredReleases.Where(d => ((d.DateOfRelease.Date <= currentRelease.DateOfRelease) || (d.Version.Major <= currentRelease.Version.Major && d.Version.Minor <= currentRelease.Version.Minor)) && d.IsHidden == false);
                return latestReleases != null ? latestReleases.LastOrDefault() : null;
            }
        }

        private Release CurrentRelease(IProductCatalogue purchaseDetails)
        {
            var currentRelease = _releaseRepository.Get(purchaseDetails.CurrentReleaseId);

            if (currentRelease == null) throw new NotFoundException("Invalid release details");

            var currentVersion = new Version(currentRelease.Version);

            return new Release
            {
                Id = currentRelease.Id,
                DateOfRelease = currentRelease.DateOfRelease,
                ProductId = currentRelease.ProductId,
                IsHidden = currentRelease.IsHidden,
                Version = currentVersion
            };
        }

        public IList<Release> OldReleasesByDate(int serialNumber)
        {
            var purchaseDetails = _serialNumberRepository.GetProductDetailsBySerialNumber(serialNumber);
            if (purchaseDetails == null) return null;

            var dateOfMaintenanceExpiry = _maintenanceContractRepository.GetDateOfExpiry(purchaseDetails.Id);

            var currentRelease = CurrentRelease(purchaseDetails);

            var isMaintenanceContractAvailable = IsMaintenanceContractAvailable(_serialNumberRepository.GetProductDetailsBySerialNumber(serialNumber).Id);
            var isProductMaintained = _productRepository.GetById(purchaseDetails.ProductId).IsMaintained;

            //if (!isMaintenanceContractAvailable) dateOfMaintenanceExpiry = currentRelease.DateOfRelease;

            return MaxReleasesByProduct(new Release(currentRelease.Version.ToString())
            {
                ProductId = currentRelease.ProductId,
                Id = currentRelease.Id,
                DateOfRelease = currentRelease.DateOfRelease,
                IsHidden = currentRelease.IsHidden
            }, isProductMaintained,
            dateOfMaintenanceExpiry, isMaintenanceContractAvailable).ToList().OrderByDescending(r => r.Version).ToList();
        }

        public IList<Release> OldReleases(int serialNumber)
        {
            var purchaseDetails = _serialNumberRepository.GetProductDetailsBySerialNumber(serialNumber);

            if (purchaseDetails == null) return null;

            var currentRelease = CurrentRelease(purchaseDetails);

            var isMaintenanceContractAvailable = IsMaintenanceContractAvailable(_serialNumberRepository.GetProductDetailsBySerialNumber(serialNumber).Id);
            var isProductMaintained = _productRepository.GetById(purchaseDetails.ProductId).IsMaintained;

            return MaxReleasesByTheProduct(new Release(currentRelease.Version.ToString())
            {
                ProductId = currentRelease.ProductId,
                Id = currentRelease.Id,
                DateOfRelease = currentRelease.DateOfRelease,
                IsHidden = currentRelease.IsHidden
            }, isProductMaintained,
            isMaintenanceContractAvailable).ToList().OrderByDescending(r => r.Version).ToList();
        }

        public Release GetById(string id)
        {
            var release = _releaseRepository.Get(id);

            return release == null ? null : new Release(release.Version)
            {
                ProductId = release.ProductId,
                DateOfRelease = release.DateOfRelease,
                Id = release.Id,
                IsHidden = release.IsHidden
                //ImageFiles = _fileManagerRepository.Blobs(_productRepository.GetById(release.ProductId).ContainerName, release.Id).Select(b => new Blob { BlobType = b.fileType, FileSize = b.fileSize }).ToList()
            };
        }

        public IList<Release> List()
        {
            try
            {
                return _releaseRepository.List().Select(s => new Release(s.Version)
                {
                    ProductId = s.ProductId,
                    DateOfRelease = s.DateOfRelease,
                    IsHidden = s.IsHidden,
                    Id = s.Id
                }).ToList()
                .OrderByDescending(r => r.Version).ToList();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw new DatabaseException();
            }
        }

        public void Update(Release release)
        {
            var isRecordExist = GetById(release.Id);

            if (isRecordExist != null)
            {
                _release.Id = release.Id;
                _release.Version = release.Version.ToString();
                _release.ProductId = release.ProductId;
                _release.DateOfRelease = release.DateOfRelease;
                _release.IsHidden = release.IsHidden;

                _releaseRepository.Update(_release);
            }
            else
            {
                throw new DuplicateEntryException("Release is doesn't exist");
            }
        }

        public void Delete(string id)
        {
            var release = _releaseRepository.Get(id);

            if (!_serialNumberRepository.IsReleaseHasProducts(id))
                _releaseRepository.Delete(release);
            else
                throw new ReferenceException("Unable to delete as registered products found");
        }

        public Stream Download(string userId, int serialNumber)
        {
            var serialDetails = _serialNumberRepository.GetProductDetailsBySerialNumber(serialNumber);
            var product = _productRepository.GetById(serialDetails.ProductId);

            var release = LatestRelease(serialNumber, serialDetails.ProductId, product.IsMaintained);

            _history.DateStamp = DateTime.UtcNow;
            _history.Id = Guid.NewGuid().ToString();
            _history.ReleaseId = release.Id;
            _history.SerialKeyId = serialDetails.Id;
            _history.UserId = userId;

            _historyRepository.Add(_history);

            return _fileManagerRepository.Download(product.ContainerName, "");
        }

        public Stream Download(string userId, string trimmedCurrentReleaseId, int serial)
        {
            var releaseDetails = _releaseRepository.GetByTrimmedId(trimmedCurrentReleaseId);

            var serialDetails = _serialNumberRepository.GetProductDetailsBySerialNumber(serial);

            var product = _productRepository.GetById(releaseDetails.ProductId);

            _history.DateStamp = DateTime.UtcNow;
            _history.Id = Guid.NewGuid().ToString();
            _history.ReleaseId = releaseDetails.Id;
            _history.SerialKeyId = serialDetails.Id;
            _history.UserId = userId;

            _historyRepository.Add(_history);

            return _fileManagerRepository.Download(product.ContainerName, releaseDetails.ImageFile);
        }

        public string ReleaseConnectionKey()
        {
            return _releaseRepository.ReleaseConnectionKey();
        }

        public string ContainerName(Release.ImageFileType imageType, string releaseId)
        {
            var release = _releaseRepository.Get(releaseId);

            if (release == null) throw new NotFoundException("Invalid release details");

            return _productRepository.GetById(release.ProductId).ContainerName + "-" + imageType.ToString().ToLower();
        }

        public Release GetReleasesByProductId(string productId)
        {
            var release = _releaseRepository.GetReleasesByProductId(productId).OrderByDescending(c => c.DateOfRelease).Where(c => c.IsHidden == false).FirstOrDefault();

            return ProcessRelease(release);
        }

        public Release ProcessRelease (IRelease releaseObject)
        {
            if (releaseObject == null) return null;

            return new Release
            {
                DateOfRelease = releaseObject.DateOfRelease,
                Id = releaseObject.Id,
                IsHidden = releaseObject.IsHidden,
                ProductId = releaseObject.ProductId,
                Version = new Version(releaseObject.Version)
            };
        }
    }
}