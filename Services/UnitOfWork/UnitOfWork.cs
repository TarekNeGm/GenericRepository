using Infrastructure.Context;
using Interfaces.Repository;
using Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Services.Repository;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace Services.UnitOfWork
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private readonly AppDbContext _context = context;
        private Hashtable? _repositories;

        public async Task<int> Complete() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
        public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
        {
            return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }
        public DbCommand CreateDbCommand()
        {
            var connection = _context.Database.GetDbConnection();

            var command = connection.CreateCommand();

            return command;
        }
        public async Task OpenConnectionAsync()
        {
            if (_context.Database.GetDbConnection().State != ConnectionState.Open)
            {
                await _context.Database.OpenConnectionAsync();
            }
        }
        public async Task CloseConnectionAsync()
        {
            if (_context.Database.GetDbConnection().State != ConnectionState.Closed)
            {
                await _context.Database.CloseConnectionAsync();
            }
        }
        public IRepository<TEntity, T> Repository<TEntity, T>() where TEntity : class where T : struct
        {
            _repositories ??= [];

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repository = new RepositoryService<TEntity, T>(_context);

                _repositories.Add(type, repository);
            }

            return (IRepository<TEntity, T>)_repositories[type]!;
        }
    }
}
