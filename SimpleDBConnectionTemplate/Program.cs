using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SimpleDatabaseConnectionTemplate.Core;
using SimpleDatabaseConnectionTemplate.Implementation.SQLite;
using SimpleDatabaseConnectionTemplate.Models;
using SimpleDatabaseConnectionTemplate.Repositories;

namespace SimpleDatabaseConnectionTemplate
{
    /// <summary>
    /// Sample entity class
    /// </summary>
    public class Person : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }

        public override string GetTableName()
        {
            return "Persons";
        }

        public override bool Validate()
        {
            // Basic validation
            if (string.IsNullOrEmpty(FirstName))
                return false;

            if (string.IsNullOrEmpty(LastName))
                return false;

            if (string.IsNullOrEmpty(Email))
                return false;

            return true;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            await SQLiteExample();
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// SQLite example
        /// </summary>
        static async Task SQLiteExample()
        {
            Console.WriteLine("SQLite Example");
            Console.WriteLine("==============");

            // Create a SQLite database
            string dbPath = "sample.db";

            // Create database file if it doesn't exist
            if (!File.Exists(dbPath))
            {
                SQLiteHelper.CreateDatabase(dbPath);
                Console.WriteLine("Created new database file");
            }
            else
            {
                Console.WriteLine("Using existing database file");
            }

            // Create a database instance
            using (var database = DatabaseFactory.CreateSQLiteDatabase(dbPath))
            {
                // Open connection
                if (await database.OpenConnectionAsync())
                {
                    Console.WriteLine("Connected to SQLite database.");

                    // Create table if not exists
                    if (!await SQLiteHelper.TableExistsAsync(database, "Persons"))
                    {
                        string createTableSql = @"
                            CREATE TABLE Persons (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                FirstName TEXT NOT NULL,
                                LastName TEXT NOT NULL,
                                Email TEXT NOT NULL,
                                DateOfBirth TEXT,
                                CreatedAt TEXT NOT NULL,
                                UpdatedAt TEXT
                            );";

                        await database.ExecuteNonQueryAsync(createTableSql);
                        Console.WriteLine("Created Persons table.");
                    }
                    else
                    {
                        Console.WriteLine("Persons table already exists.");
                    }

                    // Create repository
                    var personRepository = new GenericRepository<Person>(database);

                    // Check if there are already persons in the database
                    var existingPersons = await personRepository.GetAllAsync();
                    var personsList = existingPersons.ToList();

                    if (personsList.Count > 0)
                    {
                        Console.WriteLine($"Found {personsList.Count} existing persons in database:");
                        foreach (var p in personsList)
                        {
                            Console.WriteLine($"- {p.Id}: {p.FirstName} {p.LastName}, {p.Email}");
                        }

                        // Use first person for demonstration
                        var existingPerson = personsList[0];
                        Console.WriteLine($"\nUsing existing person with ID: {existingPerson.Id}");

                        // Update existing person
                        existingPerson.Email = $"updated.{DateTime.Now.Ticks}@example.com";
                        bool wasUpdated = await personRepository.UpdateAsync(existingPerson);
                        Console.WriteLine($"Updated person: {wasUpdated}, New email: {existingPerson.Email}");
                    }
                    else
                    {
                        Console.WriteLine("No existing persons found. Creating new person.");

                        // Create a new person
                        var person = new Person
                        {
                            FirstName = "John",
                            LastName = "Doe",
                            Email = "john.doe@example.com",
                            DateOfBirth = new DateTime(1980, 1, 1)
                        };

                        // Insert
                        person = await personRepository.CreateAsync(person);
                        Console.WriteLine($"Inserted person with ID: {person.Id}");

                        // Read
                        var retrievedPerson = await personRepository.GetByIdAsync(person.Id);
                        Console.WriteLine($"Retrieved person: {retrievedPerson.FirstName} {retrievedPerson.LastName}");

                        // Update
                        retrievedPerson.Email = "john.updated@example.com";
                        bool updated = await personRepository.UpdateAsync(retrievedPerson);
                        Console.WriteLine($"Updated person: {updated}");
                    }

                    // Read all
                    var allPersons = await personRepository.GetAllAsync();
                    Console.WriteLine("\nAll persons:");
                    foreach (var p in allPersons)
                    {
                        Console.WriteLine($"- {p.Id}: {p.FirstName} {p.LastName}, {p.Email}");
                    }

                    // Custom query example
                    var customQueryResult = await personRepository.FindAsync(
                        "SELECT * FROM Persons WHERE FirstName LIKE @FirstName",
                        new Dictionary<string, object> { { "FirstName", "J%" } }
                    );
                    Console.WriteLine("\nCustom query results (FirstName like 'J%'):");
                    foreach (var p in customQueryResult)
                    {
                        Console.WriteLine($"- {p.Id}: {p.FirstName} {p.LastName}");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to connect to SQLite database.");
                }
            }
        }
    }
}