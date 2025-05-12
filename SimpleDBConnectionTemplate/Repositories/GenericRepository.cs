using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SimpleDatabaseConnectionTemplate.Core;
using SimpleDatabaseConnectionTemplate.Implementation.MariaDB;
using SimpleDatabaseConnectionTemplate.Implementation.SQLite;
using SimpleDatabaseConnectionTemplate.Models;

namespace SimpleDatabaseConnectionTemplate.Repositories
{
    /// <summary>
    /// Generic repository implementation
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class GenericRepository<T> : IRepository<T> where T : EntityBase, new()
    {
        protected readonly IDatabase _database;
        protected readonly string _tableName;

        /// <summary>
        /// Initializes a new instance of the GenericRepository class
        /// </summary>
        /// <param name="database">Database instance</param>
        public GenericRepository(IDatabase database)
        {
            _database = database;
            var instance = new T();
            _tableName = instance.GetTableName();
        }

        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>List of entities</returns>
        public IEnumerable<T> GetAll()
        {
            string sql = $"SELECT * FROM {_tableName}";
            return MapDataTableToEntities(_database.ExecuteQuery(sql));
        }

        /// <summary>
        /// Gets all entities asynchronously
        /// </summary>
        /// <returns>List of entities</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            string sql = $"SELECT * FROM {_tableName}";
            return MapDataTableToEntities(await _database.ExecuteQueryAsync(sql));
        }

        /// <summary>
        /// Gets an entity by id
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Entity</returns>
        public T GetById(int id)
        {
            string sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
            var parameters = new Dictionary<string, object>
            {
                { "Id", id }
            };

            var dataTable = _database.ExecuteQuery(sql, parameters);
            return MapDataTableToEntities(dataTable).FirstOrDefault();
        }

        /// <summary>
        /// Gets an entity by id asynchronously
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Entity</returns>
        public async Task<T> GetByIdAsync(int id)
        {
            string sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
            var parameters = new Dictionary<string, object>
            {
                { "Id", id }
            };

            var dataTable = await _database.ExecuteQueryAsync(sql, parameters);
            return MapDataTableToEntities(dataTable).FirstOrDefault();
        }

        /// <summary>
        /// Creates a new entity
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <returns>Created entity with id</returns>
        public T Create(T entity)
        {
            if (!entity.Validate())
            {
                throw new ArgumentException("Entity validation failed.");
            }

            var properties = GetProperties(entity);

            // Remove Id property if it's 0 (let the database assign the auto-increment value)
            if (entity.Id == 0 && properties.ContainsKey("Id"))
            {
                properties.Remove("Id");
            }

            var columnNames = string.Join(", ", properties.Keys);
            var parameterNames = string.Join(", ", properties.Keys.Select(p => $"@{p}"));

            string sql = $"INSERT INTO {_tableName} ({columnNames}) VALUES ({parameterNames})";

            _database.ExecuteNonQuery(sql, properties);

            // Get the last insert id based on the database type
            long lastId;
            if (_database is SQLiteDatabase)
            {
                lastId = SQLiteHelper.GetLastInsertRowId(_database);
            }
            else if (_database is MariaDBDatabase)
            {
                lastId = MariaDBHelper.GetLastInsertId(_database);
            }
            else
            {
                // Fallback for other database types
                string idSql = $"SELECT MAX(Id) FROM {_tableName}";
                var result = _database.ExecuteScalar(idSql);
                lastId = Convert.ToInt64(result);
            }

            entity.Id = (int)lastId;
            return entity;
        }

        /// <summary>
        /// Creates a new entity asynchronously
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <returns>Created entity with id</returns>
        public async Task<T> CreateAsync(T entity)
        {
            if (!entity.Validate())
            {
                throw new ArgumentException("Entity validation failed.");
            }

            var properties = GetProperties(entity);

            // Remove Id property if it's 0 (let the database assign the auto-increment value)
            if (entity.Id == 0 && properties.ContainsKey("Id"))
            {
                properties.Remove("Id");
            }

            var columnNames = string.Join(", ", properties.Keys);
            var parameterNames = string.Join(", ", properties.Keys.Select(p => $"@{p}"));

            string sql = $"INSERT INTO {_tableName} ({columnNames}) VALUES ({parameterNames})";

            await _database.ExecuteNonQueryAsync(sql, properties);

            // Get the last insert id based on the database type
            long lastId;
            if (_database is SQLiteDatabase)
            {
                lastId = await SQLiteHelper.GetLastInsertRowIdAsync(_database);
            }
            else if (_database is MariaDBDatabase)
            {
                lastId = await MariaDBHelper.GetLastInsertIdAsync(_database);
            }
            else
            {
                // Fallback for other database types
                string idSql = $"SELECT MAX(Id) FROM {_tableName}";
                var result = await _database.ExecuteScalarAsync(idSql);
                lastId = Convert.ToInt64(result);
            }

            entity.Id = (int)lastId;
            return entity;
        }

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>True if update was successful</returns>
        public bool Update(T entity)
        {
            if (!entity.Validate())
            {
                throw new ArgumentException("Entity validation failed.");
            }

            if (entity.Id <= 0)
            {
                throw new ArgumentException("Entity Id must be greater than zero.");
            }

            entity.UpdatedAt = DateTime.Now;

            var properties = GetProperties(entity);
            var setClause = string.Join(", ", properties.Keys.Select(p => $"{p} = @{p}"));

            string sql = $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id";

            int rowsAffected = _database.ExecuteNonQuery(sql, properties);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Updates an existing entity asynchronously
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>True if update was successful</returns>
        public async Task<bool> UpdateAsync(T entity)
        {
            if (!entity.Validate())
            {
                throw new ArgumentException("Entity validation failed.");
            }

            if (entity.Id <= 0)
            {
                throw new ArgumentException("Entity Id must be greater than zero.");
            }

            entity.UpdatedAt = DateTime.Now;

            var properties = GetProperties(entity);
            var setClause = string.Join(", ", properties.Keys.Select(p => $"{p} = @{p}"));

            string sql = $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id";

            int rowsAffected = await _database.ExecuteNonQueryAsync(sql, properties);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Deletes an entity
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>True if deletion was successful</returns>
        public bool Delete(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id must be greater than zero.");
            }

            string sql = $"DELETE FROM {_tableName} WHERE Id = @Id";
            var parameters = new Dictionary<string, object>
            {
                { "Id", id }
            };

            int rowsAffected = _database.ExecuteNonQuery(sql, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Deletes an entity asynchronously
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>True if deletion was successful</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id must be greater than zero.");
            }

            string sql = $"DELETE FROM {_tableName} WHERE Id = @Id";
            var parameters = new Dictionary<string, object>
            {
                { "Id", id }
            };

            int rowsAffected = await _database.ExecuteNonQueryAsync(sql, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Finds entities by custom query
        /// </summary>
        /// <param name="query">SQL query</param>
        /// <param name="parameters">Query parameters</param>
        /// <returns>List of entities</returns>
        public IEnumerable<T> Find(string query, Dictionary<string, object> parameters = null)
        {
            var dataTable = _database.ExecuteQuery(query, parameters);
            return MapDataTableToEntities(dataTable);
        }

        /// <summary>
        /// Finds entities by custom query asynchronously
        /// </summary>
        /// <param name="query">SQL query</param>
        /// <param name="parameters">Query parameters</param>
        /// <returns>List of entities</returns>
        public async Task<IEnumerable<T>> FindAsync(string query, Dictionary<string, object> parameters = null)
        {
            var dataTable = await _database.ExecuteQueryAsync(query, parameters);
            return MapDataTableToEntities(dataTable);
        }

        /// <summary>
        /// Gets the properties of an entity as a dictionary
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Dictionary of property names and values</returns>
        protected Dictionary<string, object> GetProperties(T entity)
        {
            var properties = new Dictionary<string, object>();
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var propertyInfo in propertyInfos)
            {
                // Skip non-database properties
                if (propertyInfo.GetCustomAttribute<NonSerializedAttribute>() != null)
                {
                    continue;
                }

                var value = propertyInfo.GetValue(entity);
                properties.Add(propertyInfo.Name, value);
            }

            return properties;
        }

        /// <summary>
        /// Maps a DataTable to a list of entities
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <returns>List of entities</returns>
        protected IEnumerable<T> MapDataTableToEntities(DataTable dataTable)
        {
            var entities = new List<T>();
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (DataRow row in dataTable.Rows)
            {
                var entity = new T();

                foreach (var propertyInfo in propertyInfos)
                {
                    if (dataTable.Columns.Contains(propertyInfo.Name) && row[propertyInfo.Name] != DBNull.Value)
                    {
                        try
                        {
                            // Handle common type conversions
                            object value = row[propertyInfo.Name];
                            var targetType = propertyInfo.PropertyType;

                            // Handle nullable types
                            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                targetType = Nullable.GetUnderlyingType(targetType);
                            }

                            // Convert to the target type
                            if (targetType == typeof(DateTime) && value is string stringValue)
                            {
                                value = DateTime.Parse(stringValue);
                            }
                            else if (targetType.IsEnum && value is string enumString)
                            {
                                value = Enum.Parse(targetType, enumString);
                            }
                            else
                            {
                                value = Convert.ChangeType(value, targetType);
                            }

                            propertyInfo.SetValue(entity, value);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error mapping property {propertyInfo.Name}: {ex.Message}");
                        }
                    }
                }

                entities.Add(entity);
            }

            return entities;
        }
    }
}