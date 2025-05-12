# SimpleDBConnectionTemplate

Eine flexible, modulare Datenbank-Connector-Bibliothek für C#-Anwendungen, die SQLite und MariaDB mit CRUD-Operationen unterstützt.

## Funktionen

- Unterstützung für SQLite und MariaDB
- Generisches Repository für CRUD-Operationen
- Synchrone und asynchrone Methoden (async/await)
- Modulares Design für einfache Erweiterbarkeit
- Leichte Integration in jedes Projekt

## Projektstruktur

```
SimpleDBConnectionTemplate/
├── Core/
│   ├── IDatabase.cs             # Interface für Datenbankoperationen
│   ├── IRepository.cs           # Interface für Repository-Pattern
│   └── DatabaseFactory.cs       # Factory zur Erstellung von Datenbankinstanzen
├── Implementation/
│   ├── SQLite/
│   │   ├── SQLiteDatabase.cs    # SQLite-spezifische Implementierung
│   │   └── SQLiteHelper.cs      # SQLite-spezifische Hilfsmethoden
│   └── MariaDB/
│       ├── MariaDBDatabase.cs   # MariaDB-spezifische Implementierung
│       └── MariaDBHelper.cs     # MariaDB-spezifische Hilfsmethoden
├── Models/
│   └── EntityBase.cs            # Basisklasse für alle Entitäten
└── Repositories/
    └── GenericRepository.cs     # Generische Repository-Implementierung
```

## Erste Schritte

### Voraussetzungen

- .NET 6.0 oder höher
- SQLite und/oder MariaDB installiert (je nachdem, welche Datenbank verwendet werden soll)

### Installation

1. Füge das `SimpleDBConnectionTemplate`-Projekt zu deiner Solution hinzu.
2. Füge eine Referenz auf das `SimpleDBConnectionTemplate`-Projekt in deiner Anwendung hinzu.
3. Installiere die erforderlichen NuGet-Pakete:
   - `System.Data.SQLite.Core` für SQLite-Unterstützung
   - `MySqlConnector` für MariaDB-Unterstützung

## Grundlegende Verwendung

### Definieren einer Entität

Erstelle eine Klasse, die von `EntityBase` erbt:

```csharp
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
        // Grundlegende Validierung
        if (string.IsNullOrEmpty(FirstName))
            return false;
        
        if (string.IsNullOrEmpty(LastName))
            return false;
        
        if (string.IsNullOrEmpty(Email))
            return false;
        
        return true;
    }
}
```

### Verwenden von SQLite

```csharp
// SQLite-Datenbank erstellen
string dbPath = "sample.db";
if (!File.Exists(dbPath))
{
    SQLiteHelper.CreateDatabase(dbPath);
}

// Datenbankinstanz erstellen
using (var database = DatabaseFactory.CreateSQLiteDatabase(dbPath))
{
    // Verbindung öffnen
    if (await database.OpenConnectionAsync())
    {
        // Tabelle erstellen, falls sie nicht existiert
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
        }

        // Repository erstellen
        var personRepository = new GenericRepository<Person>(database);

        // Eine neue Person erstellen
        var person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new DateTime(1980, 1, 1)
        };

        // Einfügen
        person = await personRepository.CreateAsync(person);
        Console.WriteLine($"Person mit ID: {person.Id} eingefügt");

        // Lesen
        var retrievedPerson = await personRepository.GetByIdAsync(person.Id);
        Console.WriteLine($"Abgerufene Person: {retrievedPerson.FirstName} {retrievedPerson.LastName}");

        // Aktualisieren
        retrievedPerson.Email = "john.updated@example.com";
        bool updated = await personRepository.UpdateAsync(retrievedPerson);
        Console.WriteLine($"Person aktualisiert: {updated}");

        // Alle lesen
        var allPersons = await personRepository.GetAllAsync();
        Console.WriteLine("Alle Personen:");
        foreach (var p in allPersons)
        {
            Console.WriteLine($"- {p.Id}: {p.FirstName} {p.LastName}, {p.Email}");
        }

        // Löschen
        bool deleted = await personRepository.DeleteAsync(person.Id);
        Console.WriteLine($"Person gelöscht: {deleted}");
    }
}
```

### Verwenden von MariaDB

```csharp
// MariaDB-Datenbankinstanz erstellen
using (var database = DatabaseFactory.CreateMariaDBDatabase(
    "localhost", "test_db", "username", "password"))
{
    // Verbindung öffnen
    if (await database.OpenConnectionAsync())
    {
        // Tabelle erstellen, falls sie nicht existiert
        if (!await MariaDBHelper.TableExistsAsync(database, "Persons"))
        {
            string createTableSql = @"
                CREATE TABLE Persons (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    FirstName VARCHAR(50) NOT NULL,
                    LastName VARCHAR(50) NOT NULL,
                    Email VARCHAR(100) NOT NULL,
                    DateOfBirth DATETIME,
                    CreatedAt DATETIME NOT NULL,
                    UpdatedAt DATETIME
                );";

            await database.ExecuteNonQueryAsync(createTableSql);
        }

        // Repository erstellen
        var personRepository = new GenericRepository<Person>(database);

        // Die gleichen CRUD-Operationen wie im SQLite-Beispiel
    }
}
```

## Benutzerdefinierte Abfragen

Du kannst auch benutzerdefinierte SQL-Abfragen ausführen:

```csharp
// Benutzerdefinierte Abfrage
var customQueryResult = await personRepository.FindAsync(
    "SELECT * FROM Persons WHERE FirstName LIKE @FirstName",
    new Dictionary<string, object> { { "FirstName", "J%" } }
);
```

## Anwendungsfälle

SimpleDBConnectionTemplate ist für eine Vielzahl von Anwendungen konzipiert:

### 1. Desktop-Anwendungen

Ideal für Desktop-Anwendungen, die eine lokale Datenbank benötigen:
- Geschäftsanwendungen mit lokaler Datenspeicherung
- Offline-Tools mit Datenpersistenz
- Produktivitätsanwendungen, die Benutzereinstellungen speichern

```csharp
// Beispiel: Kundenmanagement-Desktop-Anwendung
public class CustomerApp
{
    private IDatabase _database;
    private IRepository<Customer> _customerRepository;
    
    public async Task Initialize()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                                     "MyApp", "customers.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
        
        _database = DatabaseFactory.CreateSQLiteDatabase(dbPath);
        await EnsureTablesExistAsync();
        _customerRepository = new GenericRepository<Customer>(_database);
    }
    
    private async Task EnsureTablesExistAsync()
    {
        if (!await SQLiteHelper.TableExistsAsync(_database, "Customers"))
        {
            string sql = @"CREATE TABLE Customers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT,
                Phone TEXT,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT
            );";
            await _database.ExecuteNonQueryAsync(sql);
        }
    }
    
    // Anwendungsmethoden verwenden das Repository
    public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
    {
        return await _customerRepository.FindAsync(
            "SELECT * FROM Customers WHERE Name LIKE @Search OR Email LIKE @Search",
            new Dictionary<string, object> { { "Search", $"%{searchTerm}%" } }
        );
    }
}
```

### 2. Kleine bis mittlere Webanwendungen

Für Webanwendungen, die eine einfache Datenbankschicht benötigen:
- Content-Management-Systeme
- Blogs oder persönliche Webseiten
- Interne Tools oder Admin-Panels

```csharp
// Beispiel: Einfacher Blog mit ASP.NET Core
public class BlogService
{
    private readonly IRepository<BlogPost> _blogRepository;
    
    public BlogService(IDatabase database)
    {
        _blogRepository = new GenericRepository<BlogPost>(database);
    }
    
    public async Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count)
    {
        return await _blogRepository.FindAsync(
            "SELECT * FROM BlogPosts WHERE IsPublished = @IsPublished ORDER BY PublishedDate DESC LIMIT @Count",
            new Dictionary<string, object> {
                { "IsPublished", true },
                { "Count", count }
            }
        );
    }
    
    public async Task<BlogPost> CreatePostAsync(BlogPost post)
    {
        post.CreatedAt = DateTime.Now;
        return await _blogRepository.CreateAsync(post);
    }
}

// In Startup.cs oder Program.cs
services.AddSingleton<IDatabase>(provider => {
    var config = provider.GetRequiredService<IConfiguration>();
    return DatabaseFactory.CreateMariaDBDatabase(
        config["Database:Server"],
        config["Database:Name"],
        config["Database:User"],
        config["Database:Password"]
    );
});
services.AddScoped<BlogService>();
```

### 3. Prototyping und schnelle Anwendungsentwicklung

Perfekt für die schnelle Entwicklung von Prototypen oder MVPs:
- Schnelles Erstellen von Proof-of-Concepts
- Hackathon-Projekte
- Forschungsprojekte mit Datenspeicherungsbedarf

```csharp
// Beispiel: Schnelles Prototyp für ein Inventarsystem
public class InventoryPrototype
{
    private IDatabase _database;
    private IRepository<InventoryItem> _itemRepository;
    
    public async Task RunDemo()
    {
        // Schnelle Einrichtung mit In-Memory SQLite
        _database = DatabaseFactory.CreateSQLiteDatabase(":memory:");
        await _database.OpenConnectionAsync();
        
        // Tabelle erstellen
        await _database.ExecuteNonQueryAsync(@"
            CREATE TABLE InventoryItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Quantity INTEGER NOT NULL,
                Category TEXT,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT
            );
        ");
        
        _itemRepository = new GenericRepository<InventoryItem>(_database);
        
        // Schnelles Hinzufügen von Testdaten
        await _itemRepository.CreateAsync(new InventoryItem { Name = "Laptop", Quantity = 10, Category = "Electronics" });
        await _itemRepository.CreateAsync(new InventoryItem { Name = "Desk Chair", Quantity = 5, Category = "Furniture" });
        
        // Demo-Funktionalität
        Console.WriteLine("Items by category:");
        var electronicsItems = await _itemRepository.FindAsync(
            "SELECT * FROM InventoryItems WHERE Category = @Category",
            new Dictionary<string, object> { { "Category", "Electronics" } }
        );
        
        foreach (var item in electronicsItems)
        {
            Console.WriteLine($"{item.Name}: {item.Quantity} in stock");
        }
    }
}
```

### 4. Datenmigrationstools

Nützlich für Tools, die Daten zwischen verschiedenen Systemen migrieren:
- Migrations-Scripts für Datenkonvertierungen
- ETL-Prozesse (Extract, Transform, Load)
- Datenbank-Backup-Tools

```csharp
// Beispiel: Tool zum Migrieren von Daten von SQLite zu MariaDB
public class DatabaseMigrationTool
{
    public async Task MigrateDataAsync(string sqliteDbPath, string mariaDbConnectionString)
    {
        // Quell- und Zieldatenbanken einrichten
        var sourceDb = DatabaseFactory.CreateSQLiteDatabase(sqliteDbPath);
        var targetDb = DatabaseFactory.CreateMariaDBDatabase(
            mariaDbConnectionString.Split(';')[0].Split('=')[1],  // Server
            mariaDbConnectionString.Split(';')[1].Split('=')[1],  // Database
            mariaDbConnectionString.Split(';')[2].Split('=')[1],  // User
            mariaDbConnectionString.Split(';')[3].Split('=')[1]   // Password
        );
        
        await sourceDb.OpenConnectionAsync();
        await targetDb.OpenConnectionAsync();
        
        // Tabellen in der Zieldatenbank erstellen
        var tables = await GetTablesFromSourceAsync(sourceDb);
        foreach (var table in tables)
        {
            await CreateTableInTargetAsync(sourceDb, targetDb, table);
            await MigrateTableDataAsync(sourceDb, targetDb, table);
            Console.WriteLine($"Migrated table: {table}");
        }
    }
    
    private async Task<List<string>> GetTablesFromSourceAsync(IDatabase sourceDb)
    {
        var result = await sourceDb.ExecuteQueryAsync(
            "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%';"
        );
        
        var tables = new List<string>();
        foreach (DataRow row in result.Rows)
        {
            tables.Add(row["name"].ToString());
        }
        
        return tables;
    }
    
    private async Task CreateTableInTargetAsync(IDatabase sourceDb, IDatabase targetDb, string tableName)
    {
        // Schema abrufen und für MariaDB konvertieren
        // ...
    }
    
    private async Task MigrateTableDataAsync(IDatabase sourceDb, IDatabase targetDb, string tableName)
    {
        // Daten in Batches übertragen
        // ...
    }
}
```

### 5. Microservices und kleinere Services

Für leichtgewichtige Services, die eine einfache, aber flexible Datenpersistenz benötigen:
- Interne Tools und Services
- API-Backends für einfache Anwendungen
- Monitoring- und Logging-Services

```csharp
// Beispiel: Ein einfacher Logging-Service
public class LoggingService
{
    private readonly IRepository<LogEntry> _logRepository;
    
    public LoggingService(string connectionString)
    {
        var database = DatabaseFactory.CreateMariaDBDatabase(
            connectionString.Split(';')[0].Split('=')[1],  // Server
            connectionString.Split(';')[1].Split('=')[1],  // Database
            connectionString.Split(';')[2].Split('=')[1],  // User
            connectionString.Split(';')[3].Split('=')[1]   // Password
        );
        
        database.OpenConnection();
        EnsureTableExists(database);
        
        _logRepository = new GenericRepository<LogEntry>(database);
    }
    
    private void EnsureTableExists(IDatabase database)
    {
        string sql = @"
            CREATE TABLE IF NOT EXISTS LogEntries (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                Level VARCHAR(20) NOT NULL,
                Message TEXT NOT NULL,
                Source VARCHAR(100),
                Exception TEXT,
                CreatedAt DATETIME NOT NULL,
                UpdatedAt DATETIME
            );";
            
        database.ExecuteNonQuery(sql);
    }
    
    public async Task LogInfoAsync(string message, string source = null)
    {
        await LogAsync("INFO", message, source);
    }
    
    public async Task LogErrorAsync(string message, string source = null, Exception exception = null)
    {
        await LogAsync("ERROR", message, source, exception?.ToString());
    }
    
    private async Task LogAsync(string level, string message, string source = null, string exception = null)
    {
        var logEntry = new LogEntry
        {
            Level = level,
            Message = message,
            Source = source,
            Exception = exception,
            CreatedAt = DateTime.UtcNow
        };
        
        await _logRepository.CreateAsync(logEntry);
    }
}
```

## Datenbankschema-Beispiele

### SQLite-Schema

```sql
CREATE TABLE Persons (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL,
    Email TEXT NOT NULL,
    DateOfBirth TEXT,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT
);
```

### MariaDB-Schema

```sql
CREATE TABLE Persons (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    DateOfBirth DATETIME,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME
);
```

## Erweiterung

Um Unterstützung für andere Datenbank-Engines hinzuzufügen:

1. Erstelle eine neue Implementierung des `IDatabase`-Interfaces
2. Erstelle eine Helper-Klasse mit datenbankspezifischen Hilfsfunktionen
3. Aktualisiere die `DatabaseFactory`, um den neuen Datenbanktyp zu unterstützen

## Lizenz

Dieses Projekt ist unter der MIT-Lizenz lizenziert.
