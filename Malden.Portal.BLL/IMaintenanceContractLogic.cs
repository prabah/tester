using System;
using System.Collections.Generic;

namespace Malden.Portal.BLL
{
    public interface IMaintenanceContractLogic
    {
        void Add(MaintenanceContract maintenanceContract);

        void Delete(string id);

        void Update(MaintenanceContract maintenanceContract);

        void UpdateById(MaintenanceContract maintenanceContract);

        DateTime DateOfExpiry(string serialKeyId);

        IList<MaintenanceContract> List();

        IList<MaintenanceContract> List(int serialNumber);

        MaintenanceContract Get(int serialNumber);

        MaintenanceContract Get(string id);

        bool IsMaintenanceAvailable(string serialKeyId);
    }
}