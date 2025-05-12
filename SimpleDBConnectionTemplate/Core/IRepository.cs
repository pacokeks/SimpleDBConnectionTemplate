using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleDatabaseConnectionTemplate.Models;

namespace SimpleDatabaseConnectionTemplate.Core
{
    /// <summary>
    /// Interface for generic repository
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : EntityBase
    {
        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>List of entities</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Gets all entities asynchronously
        /// </summary>
        /// <returns>List of entities</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Gets an entity by id
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Entity</returns>
        T GetById(int id);

        /// <summary>
        /// Gets an entity by id asynchronously
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Entity</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Creates a new entity
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <returns>Created entity with id</returns>
        T Create(T entity);

        /// <summary>
        /// Creates a new entity asynchronously
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <returns>Created entity with id</returns>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>True if update was successful</returns>
        bool Update(T entity);

        /// <summary>
        /// Updates an existing entity asynchronously
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>True if update was successful</returns>
        Task<bool> UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>True if deletion was successful</returns>
        bool Delete(int id);

        /// <summary>
        /// Deletes an entity asynchronously
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>True if deletion was successful</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Finds entities by custom query
        /// </summary>
        /// <param name="query">SQL query</param>
        /// <param name="parameters">Query parameters</param>
        /// <returns>List of entities</returns>
        IEnumerable<T> Find(string query, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Finds entities by custom query asynchronously
        /// </summary>
        /// <param name="query">SQL query</param>
        /// <param name="parameters">Query parameters</param>
        /// <returns>List of entities</returns>
        Task<IEnumerable<T>> FindAsync(string query, Dictionary<string, object> parameters = null);
    }
}