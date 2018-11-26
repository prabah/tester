using Malden.Portal.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Malden.Portal.BLL
{
    public class MaintenanceContractLogic : IMaintenanceContractLogic
    {
        private readonly IMaintenanceContractRepository _maintenanceContractRepository;
        private readonly IMaintenanceContract _maintenanceContract;
        private readonly IProductCatalogueLogic _serialNumberLogic;
        private readonly IProductLogic _productLogic;

        public MaintenanceContractLogic(IMaintenanceContractRepository maintenanceContractRepository,
            IMaintenanceContract maintenanceContract,
            IProductCatalogueLogic serialNumberLogic,
            IProductLogic productLogic)
        {
            _maintenanceContractRepository = maintenanceContractRepository;
            _maintenanceContract = maintenanceContract;
            _serialNumberLogic = serialNumberLogic;
            _productLogic = productLogic;
        }

        public void Add(MaintenanceContract maintenanceContract)
        {
            _maintenanceContract.DateOfExpiry = (DateTime)maintenanceContract.DateOfExpiry;
            _maintenanceContract.Id = maintenanceContract.Id;
            _maintenanceContract.SerialKeyId = maintenanceContract.SerialKeyId;
            _maintenanceContract.OrderId = maintenanceContract.OrderId;

            _maintenanceContractRepository.Add(_maintenanceContract);
        }

        public void Delete(string id)
        {
            _maintenanceContractRepository.Delete(id);
        }

        public void Update(MaintenanceContract maintenanceContract)
        {
            _maintenanceContract.DateOfExpiry = (DateTime)maintenanceContract.DateOfExpiry;
            _maintenanceContract.Id = maintenanceContract.Id;
            _maintenanceContract.SerialKeyId = maintenanceContract.SerialKeyId;
            _maintenanceContract.OrderId = maintenanceContract.OrderId;

            _maintenanceContractRepository.Update(_maintenanceContract);
        }

        public void UpdateById(MaintenanceContract maintenanceContract)
        {
            var currentMaintenence = Get(maintenanceContract.Id);

            _maintenanceContract.DateOfExpiry = (DateTime)maintenanceContract.DateOfExpiry;
            _maintenanceContract.Id = maintenanceContract.Id;
            _maintenanceContract.SerialKeyId = maintenanceContract.SerialKeyId;
            _maintenanceContract.OrderId = currentMaintenence.OrderId;
           
            _maintenanceContractRepository.UpdateById(_maintenanceContract);
        }

        DateTime IMaintenanceContractLogic.DateOfExpiry(string serialKeyId)
        {
            return _maintenanceContractRepository.GetDateOfExpiry(serialKeyId);
        }

        public IList<MaintenanceContract> List()
        {
            var maintenanceContracts = _maintenanceContractRepository.List();

            var results = from m in maintenanceContracts
                          group m by m.SerialKeyId into g
                          select new { SerialKey = g.Key, SerialNumbers = g };


            

            var list = new List<MaintenanceContract>();

            foreach (var serialKey in results)
            {
                var serialNumberDetails = _serialNumberLogic.GetByKey(serialKey.SerialKey);
                var dateOfExpiry = _maintenanceContractRepository.GetDateOfExpiry(serialKey.SerialKey);

                if (serialNumberDetails != null)
                list.Add(new MaintenanceContract
                {
                    DateOfExpiry = dateOfExpiry,
                    SerialNumber = serialNumberDetails.SerialNumber ,
                    SerialKeyId = serialNumberDetails.Id,
                    Product = _productLogic.GetById(_productLogic.GetById(serialNumberDetails.ProductId).Id)
                });
                
            }

            return list.OrderBy(s => s.SerialNumber).ToList();
        }

        public IList<MaintenanceContract> List(int serialNumber)
        {
            var serialDetails = _serialNumberLogic.GetByKey(serialNumber);

            return _maintenanceContractRepository.List(serialDetails.Id).Select(sn => new MaintenanceContract
            {
                DateOfExpiry = sn.DateOfExpiry,
                Id = sn.Id,
                SerialKeyId = sn.SerialKeyId,
                SerialNumber = _serialNumberLogic.GetByKey(sn.SerialKeyId).SerialNumber,
                Product = _productLogic.GetById(_productLogic.GetById(serialDetails.ProductId).Id)
            }).ToList().OrderByDescending(d => d.DateOfExpiry).ToList();
        }

        public MaintenanceContract Get(int serialNumber)
        {
            var serialNumberDetails = _serialNumberLogic.GetByKey(serialNumber);
            var maintenanceContract = _maintenanceContractRepository.GetBySerialKey(serialNumberDetails.Id);

            if (maintenanceContract != null)
            {
                return new MaintenanceContract
                {
                    Product = _productLogic.GetById(serialNumberDetails.ProductId),
                    DateOfExpiry = maintenanceContract.DateOfExpiry,
                    SerialKeyId = maintenanceContract.SerialKeyId,
                    SerialNumber = serialNumberDetails.SerialNumber
                };
            }
            else
                throw new NotFoundException("Maintenance contract not available");
        }

        public MaintenanceContract Get(string id)
        {
            var maintenanceContract = _maintenanceContractRepository.Get(id);
            var serialNumberDetails = _serialNumberLogic.GetByKey(maintenanceContract.SerialKeyId);

            if (maintenanceContract != null)
            {
                return new MaintenanceContract
                {
                    Product = _productLogic.GetById(serialNumberDetails.ProductId),
                    DateOfExpiry = maintenanceContract.DateOfExpiry,
                    SerialKeyId = maintenanceContract.SerialKeyId,
                    SerialNumber = serialNumberDetails.SerialNumber
                };
            }
            else
                throw new NotFoundException("Maintenance contract not available");
        }

        public bool IsMaintenanceAvailable(string serialKeyId)
        {
            return _maintenanceContractRepository.IsMaintenanceAvailable(serialKeyId);
        }
    }
}