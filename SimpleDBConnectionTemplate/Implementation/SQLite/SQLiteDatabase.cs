using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using SimpleDatabaseConnectionTemplate.Core;

namespace SimpleDatabaseConnectionTemplate.Implementation.SQLite
{
	/// <summary>
	/// SQLite database implementation
	/// </summary>
	public class SQLiteDatabase : IDatabase
	{
		private SQLiteConnection _connection;
		private readonly string _connectionString;
		private bool _isDisposed;

		/// <summary>
		/// Gets the database connection
		/// </summary>
		public IDbConnection Connection => _connection;

		/// <summary>
		/// Initializes a new instance of the SQLiteDatabase class
		/// </summary>
		/// <param name="connectionString">Connection string</param>
		public SQLiteDatabase(string connectionString)
		{
			_connectionString = connectionString;
			_connection = new SQLiteConnection(_connectionString);
		}

		/// <summary>
		/// Opens the database connection
		/// </summary>
		/// <returns>True if connection was successful</returns>
		public bool OpenConnection()
		{
			try
			{
				if (_connection.State != ConnectionState.Open)
				{
					_connection.Open();
				}
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error opening connection: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// Opens the database connection asynchronously
		/// </summary>
		/// <returns>True if connection was successful</returns>
		public async Task<bool> OpenConnectionAsync()
		{
			try
			{
				if (_connection.State != ConnectionState.Open)
				{
					await _connection.OpenAsync();
				}
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error opening connection: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// Closes the database connection
		/// </summary>
		public void CloseConnection()
		{
			if (_connection.State != ConnectionState.Closed)
			{
				_connection.Close();
			}
		}

		/// <summary>
		/// Begins a transaction
		/// </summary>
		/// <returns>The transaction object</returns>
		public IDbTransaction BeginTransaction()
		{
			if (_connection.State != ConnectionState.Open)
			{
				OpenConnection();
			}
			return _connection.BeginTransaction();
		}

		/// <summary>
		/// Creates a command with parameters
		/// </summary>
		/// <param name="sql">SQL command</param>
		/// <param name="parameters">SQL parameters</param>
		/// <returns>SQLite command</returns>
		private SQLiteCommand CreateCommand(string sql, Dictionary<string, object> parameters = null)
		{
			var command = new SQLiteCommand(sql, _connection);

			if (parameters != null)
			{
				foreach (var param in parameters)
				{
					command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
				}
			}

			return command;
		}

		/// <summary>
		/// Executes a non-query SQL command
		/// </summary>
		/// <param name="sql">SQL command</param>
		/// <param name="parameters">SQL parameters</param>
		/// <returns>Number of affected rows</returns>
		public int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null)
		{
			if (_connection.State != ConnectionState.Open)
			{
				OpenConnection();
			}

			using (var command = CreateCommand(sql, parameters))
			{
				return command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Executes a non-query SQL command asynchronously
		/// </summary>
		/// <param name="sql">SQL command</param>
		/// <param name="parameters">SQL parameters</param>
		/// <returns>Number of affected rows</returns>
		public async Task<int> ExecuteNonQueryAsync(string sql, Dictionary<string, object> parameters = null)
		{
			if (_connection.State != ConnectionState.Open)
			{
				await OpenConnectionAsync();
			}

			using (var command = CreateCommand(sql, parameters))
			{
				return await command.ExecuteNonQueryAsync();
			}
		}

		/// <summary>
		/// Executes a SQL query and returns the first column of the first row
		/// </summary>
		/// <param name="sql">SQL query</param>
		/// <param name="parameters">SQL parameters</param>
		/// <returns>First column of the first row</returns>
		public object ExecuteScalar(string sql, Dictionary<string, object> parameters = null)
		{
			if (_connection.State != ConnectionState.Open)
			{
				OpenConnection();
			}

			using (var command = CreateCommand(sql, parameters))
			{
				return command.ExecuteScalar();
			}
		}

		/// <summary>
		/// Executes a SQL query and returns the first column of the first row asynchronously
		/// </summary>
		/// <param name="sql">SQL query</param>
		/// <param name="parameters">SQL parameters</param>
		/// <returns>First column of the first row</returns>
		public async Task<object> ExecuteScalarAsync(string sql, Dictionary<string, object> parameters = null)
		{
			if (_connection.State != ConnectionState.Open)
			{
				await OpenConnectionAsync();
			}

			using (var command = CreateCommand(sql, parameters))
			{
				return await command.ExecuteScalarAsync();
			}
		}

		/// <summary>
		/// Executes a SQL query and returns a data reader
		/// </summary>
		/// <param name="sql">SQL query</param>
		/// <param name="parameters">SQL parameters</param>
		/// <returns>Data reader</returns>
		public IDataReader ExecuteReader(string sql, Dictionary<string, object> parameters = null)
		{
			if (_connection.State != ConnectionState.Open)
			{
				OpenConnection();
			}

			var command = CreateCommand(sql, parameters);
			return command.ExecuteReader(CommandBehavior.CloseConnection);
		}

		/// <summary>
		/// Executes a SQL query and returns a data reader asynchronously
		/// </summary>
		/// <param name="sql">SQL query</param>
		/// <param name="parameters">SQL parameters</param>
		/// <returns>Data reader</returns>
		public async Task<IDataReader> ExecuteReaderAsync(string sql, Dictionary<string, object> parameters = null)
		{
			if (_connection.State != ConnectionState.Open)
			{
				await OpenConnectionAsync();
			}

			var command = CreateCommand(sql, parameters);
			return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
		}

		/// <summary>
		/// Executes a SQL query and returns a data table
		/// </summary>
		/// <param name="sql">SQL query</param>
		/// <param name="parameters">SQL parameters</param>
		/// <returns>Data table</returns>
		public DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters = null)
		{
			if (_connection.State != ConnectionState.Open)
			{
				OpenConnection();
			}

			using (var command = CreateCommand(sql, parameters))
			{
				using (var adapter = new SQLiteDataAdapter(command))
				{
					var dataTable = new DataTable();
					adapter.Fill(dataTable);
					return dataTable;
				}
			}
		}

		/// <summary>
		/// Executes a SQL query and returns a data table asynchronously
		/// </summary>
		/// <param name="sql">SQL query</param>
		/// <param name="parameters">SQL parameters</param>
		/// <returns>Data table</returns>
		public async Task<DataTable> ExecuteQueryAsync(string sql, Dictionary<string, object> parameters = null)
		{
			if (_connection.State != ConnectionState.Open)
			{
				await OpenConnectionAsync();
			}

			using (var command = CreateCommand(sql, parameters))
			{
				using (var adapter = new SQLiteDataAdapter(command))
				{
					var dataTable = new DataTable();

					// SQLiteDataAdapter doesn't have async methods, so we'll use Task.Run
					await Task.Run(() => adapter.Fill(dataTable));

					return dataTable;
				}
			}
		}

		/// <summary>
		/// Disposes the database connection
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Disposes the database connection
		/// </summary>
		/// <param name="disposing">True if disposing</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
				{
					CloseConnection();
					_connection.Dispose();
				}

				_isDisposed = true;
			}
		}

		/// <summary>
		/// Finalizer
		/// </summary>
		~SQLiteDatabase()
		{
			Dispose(false);
		}
	}
}