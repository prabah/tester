using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Malden.Portal.Data.TableStorage.MaintenanceContracts
{
    public class MaintenanceContractRepository : IMaintenanceContractRepository
    {
        private readonly CloudTableClient _client;

        public MaintenanceContractRepository()
        {
            var connectionString = ConnectionStringBuilder.GetConnectionString;
            Utilities.Start(connectionString);

            var storageAccount = CloudStorageAccount.FromConfigurationSetting(connectionString);
            _client = storageAccount.CreateCloudTableClient();
        }

        public IList<IMaintenanceContract> List()
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var results = (from serialNumebrs in context.CreateQuery<MaintenanceContractEntity>(Tables.MaintenanceContracts)
                           select serialNumebrs).ToList<IMaintenanceContract>();

            return results;
        }

        public IList<IMaintenanceContract> List(string serialKey)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var results = (from serialNumebrs in context.CreateQuery<MaintenanceContractEntity>(Tables.MaintenanceContracts)
                           where (serialNumebrs.SerialKeyId == serialKey)
                           select serialNumebrs).ToList<IMaintenanceContract>();

            return results;
        }

        public void Add(IMaintenanceContract maintenanceContract)
        {
            var maintenanceContractEntity = new MaintenanceContractEntity
                                                {
                                                    Id = maintenanceContract.Id,
                                                    SerialKeyId = maintenanceContract.SerialKeyId,
                                                    DateOfExpiry = maintenanceContract.DateOfExpiry,
                                                    RowKey = KeyFactory.RowKey(maintenanceContract.Id),
                                                    PartitionKey = KeyFactory.PartitionKey(maintenanceContract.SerialKeyId),
                                                    OrderId = maintenanceContract.OrderId,
                                                    Timestamp = DateTime.Now
                                                };

            var context = _client.GetDataServiceContext();
            context.AddObject(Tables.MaintenanceContracts, maintenanceContractEntity);
            context.SaveChanges();
        }

        public IMaintenanceContract Get(string id)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);
            var results = (from serialNumebrs in context.CreateQuery<MaintenanceContractEntity>(Tables.MaintenanceContracts)
                           where (serialNumebrs.Id.Equals(id))
                           select serialNumebrs).FirstOrDefault();

            return results;
        }


        public IMaintenanceContract Get(int orderId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);
            var results = (from serialNumebrs in context.CreateQuery<MaintenanceContractEntity>(Tables.MaintenanceContracts)
                           where (serialNumebrs.OrderId.Equals(orderId))
                           select serialNumebrs).FirstOrDefault();

            return results;
        }



        public void Update(IMaintenanceContract maintenanceContract)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);
            var currentMaintenanceContract = Get(maintenanceContract.OrderId);

            if (currentMaintenanceContract != maintenanceContract)
            {
                var maintenanceContractEntity = new MaintenanceContractEntity
                {
                    Id = currentMaintenanceContract.Id,
                    SerialKeyId = currentMaintenanceContract.SerialKeyId,
                    OrderId = maintenanceContract.OrderId,
                    DateOfExpiry = maintenanceContract != null ? maintenanceContract.DateOfExpiry : currentMaintenanceContract.DateOfExpiry,
                    RowKey = KeyFactory.RowKey(currentMaintenanceContract.Id),
                    PartitionKey = KeyFactory.PartitionKey(currentMaintenanceContract.SerialKeyId),
                    Timestamp = DateTime.Now
                };

                context.AttachTo(Tables.MaintenanceContracts, maintenanceContractEntity);
                context.UpdateObject(maintenanceContractEntity);
                context.SaveChanges();
            }
        }

        public void UpdateById(IMaintenanceContract maintenanceContract)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);
            var currentMaintenanceContract = Get(maintenanceContract.Id);

            if (currentMaintenanceContract != maintenanceContract)
            {
                var maintenanceContractEntity = new MaintenanceContractEntity
                {
                    Id = currentMaintenanceContract.Id,
                    SerialKeyId = currentMaintenanceContract.SerialKeyId,
                    OrderId = currentMaintenanceContract.OrderId,
                    DateOfExpiry = maintenanceContract != null ? maintenanceContract.DateOfExpiry : currentMaintenanceContract.DateOfExpiry,
                    RowKey = KeyFactory.RowKey(currentMaintenanceContract.Id),
                    PartitionKey = KeyFactory.PartitionKey(currentMaintenanceContract.SerialKeyId),
                    Timestamp = DateTime.Now
                };

                context.AttachTo(Tables.MaintenanceContracts, maintenanceContractEntity);
                context.UpdateObject(maintenanceContractEntity);
                context.SaveChanges();
            }
        }

        public DateTime GetDateOfExpiry(string serialKeyId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var dateOfExpiry = (from contracts in context.CreateQuery<MaintenanceContractEntity>(Tables.MaintenanceContracts)
                                where (contracts.SerialKeyId.Equals(serialKeyId))
                                select contracts).ToList().OrderBy(c => c.Timestamp);

 
            return (dateOfExpiry != null && dateOfExpiry.Count() > 0) ? dateOfExpiry.LastOrDefault().DateOfExpiry : DateTime.UtcNow.Date;
        }

        public bool IsMaintenanceAvailable(string serialKeyId)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);

            var dateOfExpiry = (from contracts in context.CreateQuery<MaintenanceContractEntity>(Tables.MaintenanceContracts)
                                where (contracts.SerialKeyId.Equals(serialKeyId))
                                select contracts).ToList().OrderBy(c => c.Timestamp);



            return (dateOfExpiry != null && dateOfExpiry.Count() > 0) ? true : false;
        }

        public IMaintenanceContract GetBySerialKey(string serialKey)
        {
            var context = _client.GetDataServiceContext();
            Utilities.SetNotFoundException(context);
            var results = (from serialNumebrs in context.CreateQuery<MaintenanceContractEntity>(Tables.MaintenanceContracts)
                           where (serialNumebrs.SerialKeyId.Equals(serialKey))
                           select serialNumebrs).ToList().OrderByDescending(c => c.DateOfExpiry).FirstOrDefault();

            return results;
        }

        public void Delete(string id)
        {
            var contractEntity = Get(id);

            if (contractEntity != null)
            {
                var context = _client.GetDataServiceContext();
                context.AttachTo(Tables.MaintenanceContracts, contractEntity, "*");
                context.DeleteObject(contractEntity);
                context.SaveChanges();
            }
        }
    }
}