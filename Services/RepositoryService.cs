using Infrastructure.Context;
using Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Services
{
    public class RepositoryService<TEntity, L> : IRepository<TEntity, L> where TEntity : class where L : struct
    {
        private readonly AppDbContext _context;
        internal DbSet<TEntity> _entity;

        public RepositoryService(AppDbContext context)
        {
            _context = context;
            _entity = context.Set<TEntity>();
        }
        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> Filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> OrderBy = null, string Including = null)
        {
            IQueryable<TEntity> query = _entity;

            if (Including != null)
            {
                foreach (var item in Including.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }

            if (OrderBy != null)
            {
                return OrderBy(query);
            }

            return query;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> Filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> OrderBy = null, string Including = null, bool asNoTracking = false)
        {

            IQueryable<TEntity> query = _entity;

            if (Filter != null)
            {
                query = query.Where(Filter);
            }

            if (Including != null)
            {
                foreach (var item in Including.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }

            if (OrderBy != null)
            {
                return await OrderBy(query).ToListAsync();
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }
    }
}
