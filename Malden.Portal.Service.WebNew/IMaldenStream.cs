using System.IO;
using System.ServiceModel;

namespace Malden.Portal.Service.WebNew
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMaldenStream" in both code and config file together.
    [ServiceContract(Namespace = "urn:http://soap.malden.co.uk")]
    public interface IMaldenStream
    {
        [OperationContract]
        Stream ImageFile(string userId, int serialNumber);

        [OperationContract]
        Stream OldRelease(string userId, string id, int serial);

        [OperationContract]
        bool IsValidSerialNumber(string userName, int serialNumber);

        [OperationContract]
        bool IsProductDetailsAvailable(int serialNumber);
    }
}