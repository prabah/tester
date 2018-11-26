using Malden.Portal.BLL;
using Malden.Portal.BLL.Utilities;
using Ninject;
using System;

namespace Malden.Portal.Service.WebNew
{
    public class MaldenService : IMaldenService
    {
        private readonly IKernel _kernel;
        private readonly IMaintenanceContractLogic _maintenanceContractLogic;
        private readonly IProductCatalogueLogic _productCatalogueLogic;
        [Inject]
        public MaldenService()
        {
            _kernel = new StandardKernel();
            Bootstrapper.RegisterServices(_kernel);

            _productCatalogueLogic = _kernel.Get<IProductCatalogueLogic>();
            _maintenanceContractLogic = _kernel.Get<IMaintenanceContractLogic>();
        }

        public bool AddMaintenance(int productId, int serialNumber, DateTime dateOfExpiry, string currentRelease, int orderId, string userName, string password, bool addMaintenanceContract = true)
        {
            try
            {
                ValidateUser(userName, password);
                var product = _productCatalogueLogic.GetByKey(serialNumber);

                var id = product == null ? Guid.NewGuid().ToString() : product.Id;
                var fulfilmentProductId = ConvertToProductId(productId);
                if (fulfilmentProductId == null) throw new NotFoundException("Invalid product id");

                _productCatalogueLogic.Add(new ProductCatalogue
                {
                    ProductId = fulfilmentProductId,
                    SerialNumber = serialNumber,
                    CurrentRelease = currentRelease,
                    Id = id
                });

                if (addMaintenanceContract)
                {
                    _maintenanceContractLogic.Add(new MaintenanceContract { Id = Guid.NewGuid().ToString(), DateOfExpiry = dateOfExpiry, SerialKeyId = id, OrderId = orderId });
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw ex;
            }
        }

        public bool DeleteInactive()
        {
            try
            {
                var userLogic = _kernel.Get<IUserLogic>();
                userLogic.DeleteInactiveUsers((int) Malden.Portal.BLL.User.UserType.Customer);
                userLogic.DeleteInactiveUsers((int)Malden.Portal.BLL.User.UserType.Distributor, 30);
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                return false;
            }
        }

        public bool ExtendMaintenanace(int productId, int serialNumber, DateTime dateOfExpiry, int orderId, string userName, string password)
        {
            try
            {
                ValidateUser(userName, password);

                var serialKeyId = _productCatalogueLogic.GetIdBySerialNumber(serialNumber);

                if (serialKeyId == null) { throw new NotFoundException("Purchase details not found!"); }

                _maintenanceContractLogic.Add(new MaintenanceContract { Id = Guid.NewGuid().ToString(), SerialKeyId = serialKeyId, DateOfExpiry = dateOfExpiry, OrderId = orderId });

                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw new HandledException();
            }
        }

        public int HighestSerialNumber()
        {
            return _productCatalogueLogic.HighestSerialNumber();
        }

        public bool IsSerialNumberExists(int serialNumber, string userName, string password)
        {
            ValidateUser(userName, password);
            return _productCatalogueLogic.GetIdBySerialNumber(serialNumber) != null;
        }

        public bool UpdateMaintenanace(int productId, int serialNumber, DateTime dateOfExpiry, string currentRelease, int orderId, string userName, string password)
        {
            try
            {
                ValidateUser(userName, password);

                var serialKeyId = _productCatalogueLogic.GetIdBySerialNumber(serialNumber);

                if (serialKeyId == null) { throw new NotFoundException("Purchase details not found!"); }

                _maintenanceContractLogic.Update(new MaintenanceContract { SerialKeyId = serialKeyId, DateOfExpiry = dateOfExpiry, OrderId = orderId });

                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw new HandledException();
            }
        }

        public bool AddProductWithoutMaintenance(int productId, int serialNumber, string currentRelease, string userName, string password)
        {
            try
            {
                ValidateUser(userName, password);
                var id = Guid.NewGuid().ToString();
                var fulfilmentProductId = ConvertToProductId(productId);
                if (fulfilmentProductId == null) throw new NotFoundException("Invalid product id");

                _productCatalogueLogic.Add(new ProductCatalogue
                {
                    ProductId = fulfilmentProductId,
                    SerialNumber = serialNumber,
                    CurrentRelease = currentRelease,
                    Id = id
                });

                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw new HandledException();
            }
        }

        private string ConvertToProductId(int fulfilmentId)
        {
            var productLogic = _kernel.Get<IProductLogic>();

            return productLogic.ConvertToPortalProductId(fulfilmentId);
        }

        private void ValidateUser(string userName, string password)
        {
            var customValidator = new CustomValidator(_kernel);
            customValidator.Validate(userName, password);
        }


        public string IsValid(int productId, string currentRelease, string userName, string password)
        {
            var result = new System.Text.StringBuilder();
            result.Clear();

            var productLogic = _kernel.Get<IProductLogic>();
            var portalProduct = productLogic.GetById(productId);
            if (portalProduct == null) return "Product details not found!";


            var releaseLogic = _kernel.Get<IReleaseLogic>();
            if (releaseLogic.GetByVersion(currentRelease, portalProduct.Id) == null) return "Release details not found!";

            return "";

        }



        public bool UpdateProduct(int productId, int serialNumber, DateTime dateOfExpiry, string currentRelease, int orderId, string userName, string password)
        {
            try
            {

                ValidateUser(userName, password);
                var fulfilmentProductId = ConvertToProductId(productId);
                if (fulfilmentProductId == null) throw new NotFoundException("Invalid product id");

                _productCatalogueLogic.Update(new ProductCatalogue
                {
                    ProductId = fulfilmentProductId,
                    SerialNumber = serialNumber,
                    CurrentRelease = currentRelease,
                    Id = _productCatalogueLogic.GetIdBySerialNumber(serialNumber)
                });

                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw new HandledException();
            }
        }
    }
}