using Interfaces.Repository;
using System.Data.Common;

namespace Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        public Task<int> Complete();
        public void Dispose();
        public Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
        public DbCommand CreateDbCommand();
        public Task OpenConnectionAsync();
        public Task CloseConnectionAsync();
        public IRepository<TEntity, L> Repository<TEntity, L>() where TEntity : class where L : struct;
    }
}
