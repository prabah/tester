using Malden.Portal.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Malden.Portal.BLL
{
    public class ProductCatalogueLogic : IProductCatalogueLogic
    {
        private readonly IProductCatalogueRepository _productSerialNumberRepository;
        private readonly IReleaseRepository _releaseRepository;
        private readonly IProductCatalogue _productSerialNumber;

        public ProductCatalogueLogic(IProductCatalogueRepository productSerialNumberRepository, IProductCatalogue productSerialNumber, IReleaseRepository releaseRepository, IProductRepository productRepository)
        {
            _productSerialNumberRepository = productSerialNumberRepository;
            _productSerialNumber = productSerialNumber;
            _releaseRepository = releaseRepository;
        }

        public void Add(ProductCatalogue productSerialNumber)
        {
            if (!_productSerialNumberRepository.IsRecordExists(productSerialNumber.SerialNumber))
            {

                _productSerialNumber.Id = productSerialNumber.Id;
                _productSerialNumber.ProductId = productSerialNumber.ProductId;
                _productSerialNumber.SerialNumber = productSerialNumber.SerialNumber;

                var currentReleaseId = GetCurrentReleaseId(productSerialNumber.CurrentRelease, productSerialNumber.ProductId);

                if (currentReleaseId == null) throw new NotFoundException("Details of release not found!");

                _productSerialNumber.CurrentReleaseId = currentReleaseId;

                _productSerialNumberRepository.Add(_productSerialNumber);
            }
        }

        private string GetCurrentReleaseId(string currentRelease, string productId)
        {
            var release = _releaseRepository.GetByVersion(currentRelease, productId);

            return release != null ? release.Id : null;
        }

        public void Delete(int serialNumber)
        {
            _productSerialNumberRepository.Delete(serialNumber);
        }

        public void Update(ProductCatalogue productSerialNumber)
        {
            if (productSerialNumber.SerialNumber == 0)
            {
                throw new InvalidOperationException("Invalid operation");
            }

            _productSerialNumber.Id = productSerialNumber.Id;
            _productSerialNumber.ProductId = productSerialNumber.ProductId;
            _productSerialNumber.SerialNumber = productSerialNumber.SerialNumber;
            _productSerialNumber.CurrentReleaseId = _releaseRepository.GetByVersion(productSerialNumber.CurrentRelease, productSerialNumber.ProductId).Id;

            _productSerialNumberRepository.Update(_productSerialNumber);
        }

        public string GetIdBySerialNumber(int serialNumber)
        {
            var details = GetByKey(serialNumber);

            return details != null ? details.Id : null;
        }

        public ProductCatalogue GetByKey(int serialNumber)
        {
            var productDetails = _productSerialNumberRepository.GetProductDetailsBySerialNumber(serialNumber);

            return productDetails != null ? new ProductCatalogue
            {
                ProductId = productDetails.ProductId,
                CurrentRelease = productDetails.CurrentReleaseId,
                SerialNumber = productDetails.SerialNumber,
                Id = productDetails.Id
            } : null;
        }

        public ProductCatalogue GetByKey(string serialKey)
        {
            var productDetails = _productSerialNumberRepository.Get(serialKey);

            return productDetails != null ? new ProductCatalogue
            {
                ProductId = productDetails.ProductId,
                CurrentRelease = productDetails.CurrentReleaseId,
                SerialNumber = productDetails.SerialNumber,
                Id = productDetails.Id
            } : null;
        }

        public IList<ProductCatalogue> List()
        {
            return _productSerialNumberRepository.List().Select(sn => new ProductCatalogue
            {
                CurrentRelease = "",
                Id = sn.Id,
                ProductId = sn.ProductId,
                SerialNumber = sn.SerialNumber
            }).ToList();
        }

        public int HighestSerialNumber()
        {
            return _productSerialNumberRepository.HighestSerialNumber();
        }


        public bool IsReleaseHasProducts(string releaseId)
        {
            return _productSerialNumberRepository.IsReleaseHasProducts(releaseId);
        }
    }
}