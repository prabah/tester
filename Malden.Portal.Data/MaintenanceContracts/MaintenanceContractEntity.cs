using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System;

namespace Malden.Portal.Data.TableStorage.MaintenanceContracts
{
    public class MaintenanceContractEntity : TableServiceEntity, IMaintenanceContract
    {
        public string Id { get; set; }

        public DateTime DateOfExpiry { get; set; }

        public string SerialKeyId { get; set; }

        public int OrderId { get; set; }
    }
}