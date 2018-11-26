
using System;
using System.Collections.Generic;
using System.Linq;

namespace Malden.Portal.Data.TableStorage.Distributors
{
    public class DistributorRepository : IDistributorRepository
    {
        private readonly ITableOperations _tableOpertaions;

        public DistributorRepository() : this(new TableOperations(Tables.Distributors))
        {

        }


        public DistributorRepository(ITableOperations tableOpertaions)
        {
            _tableOpertaions = tableOpertaions == null ? new TableOperations(Tables.Distributors) : tableOpertaions;
        }

        public void Add(IDistributor distributor)
        {
            var distributorEntity = new DistributorEntity
            {
                Email = distributor.Email,
                Id = distributor.Id,
                PartitionKey = KeyFactory.PartitionKey(distributor.Id),
                RowKey = KeyFactory.RowKey(distributor.Email),
                Timestamp = DateTime.Now,
                IsActivated = false,
                IsRegistered = false,
                Token = distributor.Token
            };

           _tableOpertaions.AddToStorage(distributorEntity);
        }

        public IDistributor Get(string id)
        {
            return _tableOpertaions.Get<DistributorEntity>(c => c.Id == id);
        }

        public IList<IDistributor> List()
        {
            return _tableOpertaions.List<DistributorEntity>().ToList<IDistributor>();
        }

        public void Delete(IDistributor distributor)
        {
            _tableOpertaions.Delete<DistributorEntity>(c => c.Id == distributor.Id);
        }


        public IDistributor GetByEmail(string email)
        {
            return _tableOpertaions.Get<DistributorEntity>(c => c.Email == email);
        }

        public IDistributor GetByToken(string token)
        {
            return _tableOpertaions.Get<DistributorEntity>(c => c.Token == token);
        }

        public void Update(IDistributor distributor)
        {
            var distributorEntity = new DistributorEntity
            {
                Email = distributor.Email,
                Id = distributor.Id,
                
                Timestamp = DateTime.Now,
                IsRegistered = distributor.IsRegistered,
                IsActivated = distributor.IsActivated,
                Token = distributor.Token
            };

            
            var existing = Get(distributor.Id);
            distributorEntity.PartitionKey = KeyFactory.PartitionKey(existing.Id);
            distributorEntity.RowKey = KeyFactory.RowKey(existing.Email);

            if (existing == null) throw new ArgumentNullException("No distributor found.");

            

            _tableOpertaions.UpdateStorage((c => c.PartitionKey == distributorEntity.PartitionKey), distributorEntity);
        }
    }
}
