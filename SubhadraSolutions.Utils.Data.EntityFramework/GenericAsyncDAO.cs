using Microsoft.EntityFrameworkCore;
using SubhadraSolutions.Utils.Data.Contracts;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Data.EntityFramework;

public class GenericAsyncDAO<TEntity> : IAsyncDAO<TEntity>
    where TEntity : class
{
    /// <summary>
    /// The database set.
    /// </summary>
    private readonly DbSet<TEntity> dbSet;

    /// <summary>
    /// The context.
    /// </summary>
    private readonly DbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericAsyncDAO{TEntity}"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public GenericAsyncDAO(DbContext context)
    {
        this.context = context;
        this.dbSet = context?.Set<TEntity>();
    }

    /// <inheritdoc/>
    public async Task<TEntity> GetByIDAsync(object id)
    {
        return await this.dbSet.FindAsync(id).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        this.dbSet.Add(entity);
        await this.context.SaveChangesAsync().ConfigureAwait(false);
        return entity;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(object id)
    {
        TEntity entityToDelete = await this.dbSet.FindAsync(id).ConfigureAwait(false);
        await this.DeleteAsync(entityToDelete).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(TEntity entityToDelete)
    {
        this.dbSet.Remove(entityToDelete);
        return this.context.SaveChangesAsync();
    }
}