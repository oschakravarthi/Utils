using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Data.Contracts;

public interface IAsyncDAO<T>
    where T : class
{
    /// <summary>
    /// Gets the entity by primary key.
    /// </summary>
    /// <param name="id">The identifier. (primary key)</param>
    /// <returns>The type of the entity.</returns>
    Task<T> GetByIDAsync(object id);

    /// <summary>
    /// Inserts the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>Added Entity.</returns>
    Task<T> InsertAsync(T entity);

    /// <summary>
    /// Deletes the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task DeleteAsync(object id);

    /// <summary>
    /// Deletes the specified entity to delete.
    /// </summary>
    /// <param name="entityToDelete">The entity to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task DeleteAsync(T entityToDelete);
}