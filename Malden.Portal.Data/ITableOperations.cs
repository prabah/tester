using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Malden.Portal.Data.TableStorage
{
    public interface ITableOperations
    {
        void AddToStorage(TableServiceEntity entity);
        TableServiceEntity Get<TableServiceEntity>(Expression<Func<TableServiceEntity, bool>> filter);
        IQueryable<TableServiceEntity> List<TableServiceEntity>();
        void Delete<TableServiceEntity>(Expression<Func<TableServiceEntity, bool>> filter);
        void UpdateStorage(Expression<Func<TableServiceEntity, bool>> identityFilter, TableServiceEntity entity);
    }
}
