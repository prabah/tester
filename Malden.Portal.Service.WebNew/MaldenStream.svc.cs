using Malden.Portal.BLL;
using Ninject;
using System.IO;

namespace Malden.Portal.Service.WebNew
{
    public class MaldenStream : IMaldenStream
    {
        private readonly IKernel _kernel;

        public MaldenStream()
        {
            _kernel = new StandardKernel();
            Bootstrapper.RegisterServices(_kernel);
        }

        public Stream ImageFile(string userId, int serial)
        {
            var releaseLogic = _kernel.Get<IReleaseLogic>();

            ValidateUser(userId);

            if (!IsValidSerialNumber(userId, serial)) throw new NotFoundException("Invalid serial number!");

            if (!IsProductDetailsAvailable(serial)) throw new NotFoundException("Product deyails not found!");

            return releaseLogic.Download(userId, serial);
        }

        private void ValidateUser(string userName)
        {
            var customValidator = new CustomValidator(_kernel);
            customValidator.Validate(userName);
        }

        public bool IsValidSerialNumber(string userName, int serial)
        {
            var userPurchaseLogic = _kernel.Get<IUserPurchaseLogic>();
            return userPurchaseLogic.IsValidPurchase(userName, serial);
        }

        public bool IsProductDetailsAvailable(int serialNumber)
        {
            var productSerialLogic = _kernel.Get<IProductCatalogueLogic>();
            return productSerialLogic.GetIdBySerialNumber(serialNumber) != null;
        }

        public Stream OldRelease(string userId, string id, int serial)
        {
            var releaseLogic = _kernel.Get<IReleaseLogic>();

            ValidateUser(userId);

            return releaseLogic.Download(userId, id, serial);
        }
    }
}