using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Malden.Portal.Service.WebNew
{
    [ServiceContract(Namespace = "urn:http://soap.malden.co.uk")]
    public interface IMaldenService
    {
        [OperationContract, WebGet]
        bool AddMaintenance(int productId, int serialNumber, DateTime dateOfExpiry,string currentRelease, int orderId, string userName, string password, bool addMaintenanceContract = true);

        [OperationContract, WebGet]
        bool ExtendMaintenanace(int productId, int serialNumber, DateTime dateOfExpiry, int orderId, string userName, string password);

        [OperationContract, WebGet]
        bool UpdateMaintenanace(int productId, int serialNumber, DateTime dateOfExpiry, string currentRelease, int orderId, string userName, string password);

        [OperationContract, WebGet]
        bool AddProductWithoutMaintenance(int productId, int serialNumber, string currentRelease, string userName, string password);

        [OperationContract, WebGet]
        bool IsSerialNumberExists(int serialNumber, string userName, string password);

        [OperationContract, WebGet]
        string IsValid(int productId, string currentRelease, string userName, string password);

        [OperationContract, WebGet]
        int HighestSerialNumber();

        [OperationContract, WebGet]
        bool DeleteInactive();

        [OperationContract, WebGet]
        bool UpdateProduct(int productId, int serialNumber, DateTime dateOfExpiry, string currentRelease, int orderId, string userName, string password);

    }
}