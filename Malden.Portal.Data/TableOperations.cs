using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Malden.Portal.Data.TableStorage
{
    public class TableOperations : ITableOperations
    {
        private readonly TableServiceContext _context;
        private readonly string _tableName;

        public TableOperations(string tableName)
        {
            var connectionString = ConnectionStringBuilder.GetConnectionString;
            Utilities.Start(connectionString);

            var storageAccount = CloudStorageAccount.FromConfigurationSetting(connectionString);
            var client = storageAccount.CreateCloudTableClient();

            _tableName = tableName;
            
            _context = client.GetDataServiceContext();
            _context.IgnoreResourceNotFoundException = true;
        }

        public void AddToStorage(TableServiceEntity entity)
        {
            _context.AddObject(_tableName, entity);
            _context.SaveChanges();
        }

        public TableServiceEntity Get<TableServiceEntity>(Expression<Func<TableServiceEntity, bool>> filter)
        {
            return _context.CreateQuery<TableServiceEntity>(_tableName).Where(filter).FirstOrDefault();
        }

        public IQueryable<TableServiceEntity> List<TableServiceEntity>()
        {
            return (from u in _context.CreateQuery<TableServiceEntity>(_tableName)
                    select u).ToList<TableServiceEntity>().AsQueryable();
        }


        public void Delete<TableServiceEntity>(Expression<Func<TableServiceEntity, bool>> filter)
        {
            var entity = Get<TableServiceEntity>(filter);

            _context.Detach(entity);
            _context.AttachTo(_tableName, entity, "*");
            _context.DeleteObject(entity);
            _context.SaveChanges();
        }


        public void UpdateStorage(Expression<Func<TableServiceEntity, bool>> identityFilter, TableServiceEntity entity)
        {
            var existingEntity = (_context.CreateQuery<TableServiceEntity>(_tableName).Where(identityFilter).FirstOrDefault());

            if (existingEntity == null) throw new ArgumentException("Entity is null");

            _context.Detach(existingEntity);
            _context.AttachTo(_tableName, entity);
            _context.UpdateObject(entity);
            _context.SaveChanges();
        }
    }
}
