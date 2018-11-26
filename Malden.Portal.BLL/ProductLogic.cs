using Malden.Portal.BLL.Utilities;
using Malden.Portal.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Malden.Portal.BLL
{
    public class ProductLogic : IProductLogic
    {
        private readonly IProductRepository _productRepository;
        private readonly IReleaseRepository _releaseRepository;
        private readonly IProduct _product;

        public ProductLogic(IProductRepository productRepository, IProduct product, IReleaseRepository releaseRepository)
        {
            _productRepository = productRepository;
            _releaseRepository = releaseRepository;
            _product = product;
        }

        public void Add(Product product)
        {
            if (GetById(product.FulfilmentId) != null) throw new DuplicateEntryException("Fulfilment id already exists");

            _product.Id = Guid.NewGuid().ToString();
            _product.Name = product.Name;
            _product.FulfilmentId = product.FulfilmentId;
            _product.ContainerName = product.ContainerName;
            _product.IsMaintained = product.IsMaintained;
            _productRepository.Add(_product);
        }

        public void RemoveById(string id)
        {
            try
            {
                _productRepository.Delete(id);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw new DatabaseException();
            }
        }

        public void Update(Product product)
        {
            try
            {
                _product.Id = product.Id;
                _product.Name = product.Name;
                _product.FulfilmentId = product.FulfilmentId;
                _product.ContainerName = product.ContainerName;
                _product.IsMaintained = product.IsMaintained;

                _productRepository.Update(_product);
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw new DatabaseException();
            }
        }

        public IList<Product> List()
        {
            try
            {
                return _productRepository.List().Select(s => new Product(s.Name) { Id = s.Id, FulfilmentId = s.FulfilmentId, ContainerName = s.ContainerName }).ToList();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw new DatabaseException();
            }
        }

        public Product GetById(string id)
        {
            var productEntity = _productRepository.GetById(id);

            if (productEntity != null)
                return new Product
                {
                    ContainerName = productEntity.ContainerName,
                    Name = productEntity.Name,
                    FulfilmentId = productEntity.FulfilmentId,
                    IsMaintained = productEntity.IsMaintained,
                    Id = productEntity.Id
                };
            else return null;
        }

        public string ConvertToPortalProductId(int fulfilmentProductId)
        {
            var productId = _productRepository.ConvertToPortalProductId(fulfilmentProductId);

            return productId;
        }

        public Product GetById(int fulfilmentId)
        {
            var productEntity = _productRepository.GetByFulfilmentId(fulfilmentId);

            if (productEntity == null) return null;

            return new Product
            {
                ContainerName = productEntity.ContainerName,
                Name = productEntity.Name,
                FulfilmentId = productEntity.FulfilmentId,
                IsMaintained = productEntity.IsMaintained,
                Id = productEntity.Id
            };
        }

        public void Delete(string id)
        {
            var product = _productRepository.GetById(id);

            if (!_releaseRepository.IsAnyReleasesForProduct(id))
                _productRepository.Delete(product);
            else
                throw new ReferenceException("Unable to delete as releases found");
        }
    }
}