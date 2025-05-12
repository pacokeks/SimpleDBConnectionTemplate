using System;
using System.Threading.Tasks;
using SimpleDatabaseConnectionTemplate.Core;

namespace SimpleDatabaseConnectionTemplate.Implementation.MariaDB
{
	/// <summary>
	/// Helper class for MariaDB operations
	/// </summary>
	public static class MariaDBHelper
	{
		/// <summary>
		/// Checks if a table exists in the database
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <param name="tableName">Table name</param>
		/// <returns>True if the table exists</returns>
		public static bool TableExists(IDatabase database, string tableName)
		{
			string sql = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = @tableName;";
			var parameters = new System.Collections.Generic.Dictionary<string, object>
			{
				{ "tableName", tableName }
			};

			var result = database.ExecuteScalar(sql, parameters);
			return Convert.ToInt32(result) > 0;
		}

		/// <summary>
		/// Checks if a table exists in the database asynchronously
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <param name="tableName">Table name</param>
		/// <returns>True if the table exists</returns>
		public static async Task<bool> TableExistsAsync(IDatabase database, string tableName)
		{
			string sql = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = @tableName;";
			var parameters = new System.Collections.Generic.Dictionary<string, object>
			{
				{ "tableName", tableName }
			};

			var result = await database.ExecuteScalarAsync(sql, parameters);
			return Convert.ToInt32(result) > 0;
		}

		/// <summary>
		/// Gets the last insert ID
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <returns>Last insert ID</returns>
		public static long GetLastInsertId(IDatabase database)
		{
			string sql = "SELECT LAST_INSERT_ID();";
			var result = database.ExecuteScalar(sql);
			return Convert.ToInt64(result);
		}

		/// <summary>
		/// Gets the last insert ID asynchronously
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <returns>Last insert ID</returns>
		public static async Task<long> GetLastInsertIdAsync(IDatabase database)
		{
			string sql = "SELECT LAST_INSERT_ID();";
			var result = await database.ExecuteScalarAsync(sql);
			return Convert.ToInt64(result);
		}

		/// <summary>
		/// Gets database engine version
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <returns>Database version</returns>
		public static string GetDatabaseVersion(IDatabase database)
		{
			string sql = "SELECT VERSION();";
			var result = database.ExecuteScalar(sql);
			return result?.ToString();
		}

		/// <summary>
		/// Gets database engine version asynchronously
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <returns>Database version</returns>
		public static async Task<string> GetDatabaseVersionAsync(IDatabase database)
		{
			string sql = "SELECT VERSION();";
			var result = await database.ExecuteScalarAsync(sql);
			return result?.ToString();
		}

		/// <summary>
		/// Gets the current database name
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <returns>Database name</returns>
		public static string GetDatabaseName(IDatabase database)
		{
			string sql = "SELECT DATABASE();";
			var result = database.ExecuteScalar(sql);
			return result?.ToString();
		}

		/// <summary>
		/// Gets the current database name asynchronously
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <returns>Database name</returns>
		public static async Task<string> GetDatabaseNameAsync(IDatabase database)
		{
			string sql = "SELECT DATABASE();";
			var result = await database.ExecuteScalarAsync(sql);
			return result?.ToString();
		}

		/// <summary>
		/// Gets the size of a table in bytes
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <param name="tableName">Table name</param>
		/// <returns>Table size in bytes</returns>
		public static long GetTableSize(IDatabase database, string tableName)
		{
			string sql = @"
                SELECT data_length + index_length AS size
                FROM information_schema.tables
                WHERE table_schema = DATABASE()
                AND table_name = @tableName;";

			var parameters = new System.Collections.Generic.Dictionary<string, object>
			{
				{ "tableName", tableName }
			};

			var result = database.ExecuteScalar(sql, parameters);
			return result != null ? Convert.ToInt64(result) : 0;
		}

		/// <summary>
		/// Gets the size of a table in bytes asynchronously
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <param name="tableName">Table name</param>
		/// <returns>Table size in bytes</returns>
		public static async Task<long> GetTableSizeAsync(IDatabase database, string tableName)
		{
			string sql = @"
                SELECT data_length + index_length AS size
                FROM information_schema.tables
                WHERE table_schema = DATABASE()
                AND table_name = @tableName;";

			var parameters = new System.Collections.Generic.Dictionary<string, object>
			{
				{ "tableName", tableName }
			};

			var result = await database.ExecuteScalarAsync(sql, parameters);
			return result != null ? Convert.ToInt64(result) : 0;
		}

		/// <summary>
		/// Optimizes a table
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <param name="tableName">Table name</param>
		/// <returns>True if optimization was successful</returns>
		public static bool OptimizeTable(IDatabase database, string tableName)
		{
			try
			{
				string sql = $"OPTIMIZE TABLE {tableName};";
				database.ExecuteNonQuery(sql);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error optimizing table: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// Optimizes a table asynchronously
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <param name="tableName">Table name</param>
		/// <returns>True if optimization was successful</returns>
		public static async Task<bool> OptimizeTableAsync(IDatabase database, string tableName)
		{
			try
			{
				string sql = $"OPTIMIZE TABLE {tableName};";
				await database.ExecuteNonQueryAsync(sql);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error optimizing table: {ex.Message}");
				return false;
			}
		}
	}
}