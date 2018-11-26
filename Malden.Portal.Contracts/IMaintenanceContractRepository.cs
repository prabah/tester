using System;
using System.Collections.Generic;

namespace Malden.Portal.Data
{
    public interface IMaintenanceContractRepository
    {
        void Add(IMaintenanceContract maintenanceContract);

        void Delete(string id);

        void Update(IMaintenanceContract maintenanceContract);

        void UpdateById(IMaintenanceContract maintenanceContract);

        DateTime GetDateOfExpiry(string serialKeyId);

        IList<IMaintenanceContract> List();

        IList<IMaintenanceContract> List(string serialKey);

        IMaintenanceContract Get(string id);

        IMaintenanceContract GetBySerialKey(string serialKey);

        bool IsMaintenanceAvailable(string serialKeyId);
    }
}