using Infrastructure.Context;
using Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Services.Repository
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
        public void Delete(TEntity Obj)
        {
            _entity.Remove(Obj);
        }
        void IRepository<TEntity, L>.SoftDelete(TEntity Obj)
        {
            typeof(TEntity).GetProperty("IsDeleted")?.SetValue(Obj, true);
            typeof(TEntity).GetProperty("IsActive")?.SetValue(Obj, false);
        }
        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            _entity.RemoveRange(entities);
        }
        public void Activate(TEntity Obj)
        {
            typeof(TEntity).GetProperty("IsDeleted")?.SetValue(Obj, false);
            typeof(TEntity).GetProperty("IsActive")?.SetValue(Obj, true);
        }
        public void Deactivate(TEntity Obj) => typeof(TEntity).GetProperty("IsActive")?.SetValue(Obj, false);
        public async Task<TEntity> GetByIdAsync(L Id)
        {
            return await _entity.FindAsync(Id);
        }
        public async Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> Filter)
        {
            return await _entity.AnyAsync(Filter);
        }
        public async Task<TEntity> GetObjAsync(Expression<Func<TEntity, bool>> Filter = null, string Including = null)
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
            return await query.FirstOrDefaultAsync();
        }
        public void Update(TEntity Obj)
        {
            //_context.ChangeTracker.Clear();
            _context.Entry(Obj).State = EntityState.Modified;
        }
        public async Task AddRangAsync(List<TEntity> Objs)
        {
            await _entity.AddRangeAsync(Objs);
        }
        public void UpdateRange(List<TEntity> Obj)
        {
            _entity.UpdateRange(Obj);
        }
        public void ClearChangeTracker() => _context.ChangeTracker.Clear();
        public async Task AddAsync(TEntity Obj)
        {
            //_context.ChangeTracker.Clear();

            await _entity.AddAsync(Obj);
        }
        public IQueryable<TEntity> Query()
        {
            IQueryable<TEntity> query = _entity;

            query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            query = query.Where(e => EF.Property<bool>(e, "IsActive") == true);

            return query;
        }
    }
}
