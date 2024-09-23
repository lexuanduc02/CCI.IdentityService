using CCI.Domain.Contractors;
using System.Linq.Expressions;

namespace CCI.Repository.Contractors
{
    public interface IRepository<TEntity, in TKey> where TEntity : IEntity<TKey>
    {
        Task<TEntity?> GetById(TKey id, params Expression<Func<TEntity, object>>[] includes);
        Task<List<TEntity>?> FindByAsync(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity?> FirstOrDefaultByAsync(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);
    }

}
