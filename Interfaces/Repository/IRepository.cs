using System.Linq.Expressions;

namespace Interfaces.Repository
{
    public interface IRepository<TEntity, T> where TEntity : class where T : struct
    {
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> Filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> OrderBy = null, string Including = null);

        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> Filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> OrderBy = null, string Including = null, bool asNoTracking = false);

        Task<TEntity> GetByIdAsync(T Id);
        Task<TEntity> GetObjAsync(Expression<Func<TEntity, bool>> Filter = null, string Including = null);
        Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> Filter);
        Task AddAsync(TEntity Obj);
        void ClearChangeTracker();
        Task AddRangAsync(List<TEntity> Objs);
        void Update(TEntity Obj);
        void UpdateRange(List<TEntity> Obj);
        void Delete(TEntity Obj);
        void SoftDelete(TEntity Obj);
        void DeleteRange(IEnumerable<TEntity> entities);
        void Activate(TEntity Obj);
        void Deactivate(TEntity Obj);
        IQueryable<TEntity> Query();
    }

}
