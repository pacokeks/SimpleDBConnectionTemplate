using System;
using System.IO;
using System.Threading.Tasks;
using SimpleDatabaseConnectionTemplate.Core;

namespace SimpleDatabaseConnectionTemplate.Implementation.SQLite
{
	/// <summary>
	/// Helper class for SQLite operations
	/// </summary>
	public static class SQLiteHelper
	{
		/// <summary>
		/// Creates a new SQLite database file
		/// </summary>
		/// <param name="databasePath">Path to the database file</param>
		/// <returns>True if the database was created successfully</returns>
		public static bool CreateDatabase(string databasePath)
		{
			try
			{
				if (!File.Exists(databasePath))
				{
					System.Data.SQLite.SQLiteConnection.CreateFile(databasePath);
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error creating database: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// Checks if a table exists in the database
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <param name="tableName">Table name</param>
		/// <returns>True if the table exists</returns>
		public static bool TableExists(IDatabase database, string tableName)
		{
			string sql = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName;";
			var parameters = new System.Collections.Generic.Dictionary<string, object>
			{
				{ "tableName", tableName }
			};

			var result = database.ExecuteScalar(sql, parameters);
			return result != null;
		}

		/// <summary>
		/// Checks if a table exists in the database asynchronously
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <param name="tableName">Table name</param>
		/// <returns>True if the table exists</returns>
		public static async Task<bool> TableExistsAsync(IDatabase database, string tableName)
		{
			string sql = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName;";
			var parameters = new System.Collections.Generic.Dictionary<string, object>
			{
				{ "tableName", tableName }
			};

			var result = await database.ExecuteScalarAsync(sql, parameters);
			return result != null;
		}

		/// <summary>
		/// Gets the last insert row ID
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <returns>Last insert row ID</returns>
		public static long GetLastInsertRowId(IDatabase database)
		{
			string sql = "SELECT last_insert_rowid();";
			var result = database.ExecuteScalar(sql);
			return Convert.ToInt64(result);
		}

		/// <summary>
		/// Gets the last insert row ID asynchronously
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <returns>Last insert row ID</returns>
		public static async Task<long> GetLastInsertRowIdAsync(IDatabase database)
		{
			string sql = "SELECT last_insert_rowid();";
			var result = await database.ExecuteScalarAsync(sql);
			return Convert.ToInt64(result);
		}

		/// <summary>
		/// Vacuums the database to optimize storage
		/// </summary>
		/// <param name="database">Database instance</param>
		public static void VacuumDatabase(IDatabase database)
		{
			database.ExecuteNonQuery("VACUUM;");
		}

		/// <summary>
		/// Vacuums the database to optimize storage asynchronously
		/// </summary>
		/// <param name="database">Database instance</param>
		public static async Task VacuumDatabaseAsync(IDatabase database)
		{
			await database.ExecuteNonQueryAsync("VACUUM;");
		}

		/// <summary>
		/// Backs up the database to a file
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <param name="backupPath">Path to the backup file</param>
		/// <returns>True if the backup was successful</returns>
		public static bool BackupDatabase(IDatabase database, string backupPath)
		{
			try
			{
				database.OpenConnection();
				string sql = $"BACKUP DATABASE TO '{backupPath}';";
				database.ExecuteNonQuery(sql);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error backing up database: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// Backs up the database to a file asynchronously
		/// </summary>
		/// <param name="database">Database instance</param>
		/// <param name="backupPath">Path to the backup file</param>
		/// <returns>True if the backup was successful</returns>
		public static async Task<bool> BackupDatabaseAsync(IDatabase database, string backupPath)
		{
			try
			{
				await database.OpenConnectionAsync();
				string sql = $"BACKUP DATABASE TO '{backupPath}';";
				await database.ExecuteNonQueryAsync(sql);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error backing up database: {ex.Message}");
				return false;
			}
		}
	}
}