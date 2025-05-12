using System;
using SimpleDatabaseConnectionTemplate.Implementation.SQLite;
using SimpleDatabaseConnectionTemplate.Implementation.MariaDB;

namespace SimpleDatabaseConnectionTemplate.Core
{
    /// <summary>
    /// Factory to create database instances
    /// </summary>
    public static class DatabaseFactory
    {
        /// <summary>
        /// Database type enum
        /// </summary>
        public enum DatabaseType
        {
            SQLite,
            MariaDB
        }

        /// <summary>
        /// Creates a database instance
        /// </summary>
        /// <param name="type">Database type</param>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Database instance</returns>
        public static IDatabase CreateDatabase(DatabaseType type, string connectionString)
        {
            switch (type)
            {
                case DatabaseType.SQLite:
                    return new SQLiteDatabase(connectionString);
                case DatabaseType.MariaDB:
                    return new MariaDBDatabase(connectionString);
                default:
                    throw new ArgumentException($"Database type {type} is not supported.");
            }
        }

        /// <summary>
        /// Creates an SQLite database instance
        /// </summary>
        /// <param name="databasePath">Path to the SQLite database file</param>
        /// <returns>SQLite database instance</returns>
        public static IDatabase CreateSQLiteDatabase(string databasePath)
        {
            string connectionString = $"Data Source={databasePath};Version=3;";
            return new SQLiteDatabase(connectionString);
        }

        /// <summary>
        /// Creates a MariaDB database instance
        /// </summary>
        /// <param name="server">Server address</param>
        /// <param name="database">Database name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="port">Port number (default: 3306)</param>
        /// <returns>MariaDB database instance</returns>
        public static IDatabase CreateMariaDBDatabase(string server, string database, string username, string password, int port = 3306)
        {
            string connectionString = $"Server={server};Database={database};User={username};Password={password};Port={port};";
            return new MariaDBDatabase(connectionString);
        }
    }
}