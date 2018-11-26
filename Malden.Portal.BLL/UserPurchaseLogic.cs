using Malden.Portal.BLL.Utilities;
using Malden.Portal.Data;
using System;
using System.Collections.Generic;

namespace Malden.Portal.BLL
{
    public class UserPurchaseLogic : IUserPurchaseLogic
    {
        private readonly IUserPurchaseRepository _userPurchaseRepository;
        private readonly IProductRepository _productRepository;
        private readonly IReleaseLogic _releaseLogic;
        private readonly IUserPurchase _userPurchase;
        private readonly IProductCatalogueLogic _productSerialNumberLogic;
        private readonly IMaintenanceContractLogic _maintenanceContractLogic;
        private readonly IProductLogic _productLogic;
        private readonly IUserLogic _userLogic;
        private readonly IBlobManagerLogic _blobLogic;

        public UserPurchaseLogic(IUserPurchaseRepository userPurchaseRepository,
            IProductRepository productRepository,
            IReleaseLogic releaseLogic,
            IUserPurchase userPurchase,
            IProductCatalogueLogic productSerialNumberLogic,
            IMaintenanceContractLogic maintenanceContractLogic,
            IProductLogic producLogic,
            IUserLogic userLogic,
            IBlobManagerLogic blobLogic
            )
        {
            _userPurchaseRepository = userPurchaseRepository;
            _productRepository = productRepository;
            _releaseLogic = releaseLogic;
            _userPurchase = userPurchase;
            _productSerialNumberLogic = productSerialNumberLogic;
            _maintenanceContractLogic = maintenanceContractLogic;
            _productLogic = producLogic;
            _userLogic = userLogic;
            _blobLogic = blobLogic;
        }

        public void Add(UserPurchase userPurchase, string email)
        {
            userPurchase.RegistrationCode = userPurchase.RegistrationCode.Length >= 11 ? userPurchase.RegistrationCode : Format5CharRegCode(userPurchase.RegistrationCode);

            var serialNumber = ConvertToKey(userPurchase.RegistrationCode);

            var userPurchaseInfo = _userPurchaseRepository.GetBySerialNumber(serialNumber, email);

            var productId = _productSerialNumberLogic.GetByKey(serialNumber).ProductId;

            if (!IsValid(userPurchaseInfo, productId, userPurchase.RegistrationCode)) return;

            _userPurchase.RegistrationCode = userPurchase.RegistrationCode;
            _userPurchase.ProductId = productId;
            _userPurchase.UserId = userPurchase.UserId;
            _userPurchase.SerialNumber = serialNumber;
            _userPurchaseRepository.Add(_userPurchase, Guid.NewGuid().ToString());

            _userLogic.IncreaseRegisteredProductCounter(email);
        }

        private bool IsValid(IUserPurchase userPurchase, string productId, string registrationCode)
        {
            var serialNumberDetails = _productSerialNumberLogic.GetIdBySerialNumber(ConvertToKey(registrationCode));

            if (serialNumberDetails == null)
            {
                throw new NotFoundException("Serial number entered is invalid");
            }

            if (userPurchase != null)
            {
                throw new DuplicateEntryException("Serial number already exists");
            }

            if (string.IsNullOrEmpty(productId))
            {
                throw new NotFoundException("Product details for the serial number is not found");
            }

            var releaseInfo = _productSerialNumberLogic.GetByKey(ConvertToKey(registrationCode));

            if (_releaseLogic.GetById(releaseInfo.CurrentRelease) == null)
            {
                throw new NotFoundException("Release details for the serial number is not found");
            }

            return true;
        }

        

        public IList<UserPurchase> List(string email)
        {
            var userPurchases = _userPurchaseRepository.UserPurchasesByEmail(email);

            var list = new List<UserPurchase>();

            foreach (var u in userPurchases)
            {
                var userProduct = _productRepository.GetById(u.ProductId);
                var serialNumber = ConvertToKey(u.RegistrationCode);
                var availbaleRelease = _releaseLogic.LatestReleaseByDate(serialNumber, u.ProductId, userProduct.IsMaintained);
                var serialNumberDetails = _productSerialNumberLogic.GetByKey(serialNumber);

                var currentRelease = _releaseLogic.GetById(serialNumberDetails.CurrentRelease).Version.ToString();

                var dateOfExpiry = _maintenanceContractLogic.DateOfExpiry(serialNumberDetails.Id);

                var availableReleaseFiles = new List<CloudFile>();

                if (availbaleRelease != null)
                {
                    //availableReleaseFiles = CloudFilesModel.FilteredFiles(availbaleRelease.Id, _releaseLogic, _productLogic, _blobLogic);
                    //availbaleRelease.ReleaseImageFiles = availableReleaseFiles;
                    availbaleRelease.ReleaseImageFiles = _blobLogic.CloudFiles(availbaleRelease.Id);
                }

                var downloadableReleases = _releaseLogic.GetMajorReleases(_releaseLogic.OldReleasesByDate(serialNumber));
                
                list.Add(new UserPurchase
                {
                    PurchaseId = u.Id,
                    Product = new Product { Name = userProduct.Name, Id = userProduct.Id, IsMaintained = userProduct.IsMaintained },
                    SerialNumber = serialNumber,
                    AvailableRelease = availbaleRelease,
                    OldReleases = downloadableReleases,
                    CurrentRelease = currentRelease != null ? currentRelease : "",
                    MaintenanceExpiryDate = userProduct.IsMaintained ? dateOfExpiry : System.DateTime.UtcNow,
                    IsMaintenanceAvailable = _maintenanceContractLogic.IsMaintenanceAvailable(serialNumberDetails.Id)
                });
            }

            return list;
        }

        private int ConvertToKey(string registrationCode)
        {
            int serialKey;
            var isValidSerialKey = int.TryParse(registrationCode.Substring(0, 5), out serialKey);

            return isValidSerialKey ? serialKey : 0;
        }

        public UserPurchase GetById(string id)
        {
            var userPurchaseDetails = _userPurchaseRepository.Get(id);

            return new UserPurchase(userPurchaseDetails.RegistrationCode, userPurchaseDetails.UserId)
            {
                ProductId = userPurchaseDetails.ProductId,
                SerialNumber = ConvertToKey(userPurchaseDetails.RegistrationCode),
                RegistrationCode = userPurchaseDetails.RegistrationCode
            };
        }

        public void Update(UserPurchase userPurchase, string email)
        {
            var currentUserPurchase = _userPurchaseRepository.GetBySerialNumber(userPurchase.SerialNumber, email);

            if (currentUserPurchase != null)
            {
                var productId = userPurchase.ProductId;

                _userPurchase.RegistrationCode = currentUserPurchase.RegistrationCode;
                _userPurchase.ProductId = productId;
                _userPurchase.UserId = userPurchase.UserId;
                _userPurchase.Id = currentUserPurchase.Id;
                _userPurchase.SerialNumber = userPurchase.SerialNumber;

                _userPurchaseRepository.Update(_userPurchase, email);
            }
        }

        public bool IsValidPurchase(string userName, int serialNumber)
        {
            return _userPurchaseRepository.IsValidSerialNumber(userName, serialNumber);
        }

        private string Format5CharRegCode(string registrationCode)
        {
            int nonMaintainedSerialNumber;
            Int32.TryParse(registrationCode, out nonMaintainedSerialNumber);
            var purchaseDetails = _productSerialNumberLogic.GetByKey(nonMaintainedSerialNumber);
            //if (_productRepository.GetById(purchaseDetails.ProductId).IsMaintained) return false;

            return nonMaintainedSerialNumber.ToString("D5") + "-" + (PasswordResolver.CalculateMD5Hash(nonMaintainedSerialNumber.ToString())).Substring(0, 5).ToUpper();

        }

        public bool IsValidSerialNumber(string email, string registrationCode, bool fullValidation = true)
        {
            var splitter = registrationCode.IndexOf("-");


            if (splitter <= 0)
            {
                int nonMaintainedSerialNumber;
                Int32.TryParse(registrationCode, out nonMaintainedSerialNumber);
                var purchaseDetails = _productSerialNumberLogic.GetByKey(nonMaintainedSerialNumber);

                if (purchaseDetails == null) return false;

                var productDetails = _productRepository.GetById(purchaseDetails.ProductId);
                
                if (productDetails == null) return false;
                if (_productRepository.GetById(purchaseDetails.ProductId).IsMaintained) return false;
                
                
                registrationCode += "-" + (PasswordResolver.CalculateMD5Hash(nonMaintainedSerialNumber.ToString())).Substring(0, 5).ToUpper();
                splitter = registrationCode.IndexOf("-");
            }

            //if (splitter <= 0) return false; 
            
            var serialString = registrationCode.Substring(0, splitter);
            var hashedString = registrationCode.Substring(splitter + 1, registrationCode.Length - serialString.Length - 1);

            int convertedSerialNumber;

            Int32.TryParse(serialString, out convertedSerialNumber);

            if (convertedSerialNumber <= 1) return false;

            if (fullValidation)
            {
                if (_productSerialNumberLogic.GetByKey(convertedSerialNumber) == null) return false;
            }

            return (hashedString == ((PasswordResolver.CalculateMD5Hash(convertedSerialNumber.ToString())).Substring(0, 5)).ToUpper());
        }

        public IList<Release> LatestReleasesForAllProducts()
        {
            var products = _productRepository.List();
            var releases = new List<Release>();

            foreach (var product in products)
            {
                var release = _releaseLogic.GetReleasesByProductId(product.Id);

                if (release != null)
                {
                    var cloudFiles = _blobLogic.CloudFiles(release.Id);
                    if (cloudFiles.Count > 0)
                    {
                        release.ReleaseImageFiles = _blobLogic.CloudFiles(release.Id);

                        releases.Add(release);
                    }
                }
            }

            return releases;
        }
    }


}