using System.Linq.Expressions;
using CCI.Domain.Contractors;
using CCI.Domain.EF;
using Microsoft.EntityFrameworkCore;

namespace CCI.Repository;

public abstract class Repository<TEntity, TKey> where TEntity : class, IEntity<TKey>, new()
{
    protected readonly DataContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected Repository(DataContext context)
    {
        Context = context;
        DbSet = Context.Set<TEntity>();
    }

    public async Task<TEntity?> GetById(TKey id, params Expression<Func<TEntity, object>>[] includes)
    {
        var result = await FindByAsync(o => o.Id != null && o.Id.Equals(id), includes);
        return result.FirstOrDefault();
    }

    public async Task<List<TEntity>?> FindByAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();
        query = includes.Aggregate(query, (current, include) => current.Include(include));

        var result = await query.Where(predicate).ToListAsync();
        return result;
    }

    public async Task<TEntity?> FirstOrDefaultByAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();
        query = includes.Aggregate(query, (current, include) => current.Include(include));

        var result = await query.Where(predicate).FirstOrDefaultAsync();
        return result;
    }

}
