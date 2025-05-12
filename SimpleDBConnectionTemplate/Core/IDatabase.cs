using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SimpleDatabaseConnectionTemplate.Core
{
    /// <summary>
    /// Interface for database operations
    /// </summary>
    public interface IDatabase : IDisposable
    {
        /// <summary>
        /// Gets the database connection
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// Opens the database connection
        /// </summary>
        /// <returns>True if connection was successful</returns>
        bool OpenConnection();

        /// <summary>
        /// Opens the database connection asynchronously
        /// </summary>
        /// <returns>True if connection was successful</returns>
        Task<bool> OpenConnectionAsync();

        /// <summary>
        /// Closes the database connection
        /// </summary>
        void CloseConnection();

        /// <summary>
        /// Begins a transaction
        /// </summary>
        /// <returns>The transaction object</returns>
        IDbTransaction BeginTransaction();

        /// <summary>
        /// Executes a non-query SQL command
        /// </summary>
        /// <param name="sql">SQL command</param>
        /// <param name="parameters">SQL parameters</param>
        /// <returns>Number of affected rows</returns>
        int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Executes a non-query SQL command asynchronously
        /// </summary>
        /// <param name="sql">SQL command</param>
        /// <param name="parameters">SQL parameters</param>
        /// <returns>Number of affected rows</returns>
        Task<int> ExecuteNonQueryAsync(string sql, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Executes a SQL query and returns the first column of the first row
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <param name="parameters">SQL parameters</param>
        /// <returns>First column of the first row</returns>
        object ExecuteScalar(string sql, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Executes a SQL query and returns the first column of the first row asynchronously
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <param name="parameters">SQL parameters</param>
        /// <returns>First column of the first row</returns>
        Task<object> ExecuteScalarAsync(string sql, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Executes a SQL query and returns a data reader
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <param name="parameters">SQL parameters</param>
        /// <returns>Data reader</returns>
        IDataReader ExecuteReader(string sql, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Executes a SQL query and returns a data reader asynchronously
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <param name="parameters">SQL parameters</param>
        /// <returns>Data reader</returns>
        Task<IDataReader> ExecuteReaderAsync(string sql, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Executes a SQL query and returns a data table
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <param name="parameters">SQL parameters</param>
        /// <returns>Data table</returns>
        DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Executes a SQL query and returns a data table asynchronously
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <param name="parameters">SQL parameters</param>
        /// <returns>Data table</returns>
        Task<DataTable> ExecuteQueryAsync(string sql, Dictionary<string, object> parameters = null);
    }
}