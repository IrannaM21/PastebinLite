# PastebinLite

A lightweight Pastebin-like service built with ASP.NET Core that allows users to create and share text snippets with optional expiration and view limits.

## Running the App Locally

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL 12+ installed and running
- A PostgreSQL user with database creation privileges

### Setup Steps

1. **Create PostgreSQL Database**
   ```bash
   psql -U postgres
   CREATE DATABASE pastebin_db;
   CREATE USER pastebinlite_user WITH PASSWORD 'Pastebin@123';
   GRANT ALL PRIVILEGES ON DATABASE pastebin_db TO pastebinlite_user;
   ```

2. **Configure Connection String** (if needed)
   
   The default connection string is in `appsettings.json`:
   ```json
   "Postgres": "Host=localhost;Port=5432;Database=pastebin_db;Username=pastebinlite_user;Password=Pastebin@123"
   ```

3. **Apply Database Migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

5. **Access the Application**
   - API: `https://localhost:7xxx/api/pastes`
   - Swagger UI: `https://localhost:7xxx/swagger` (Development only)
   - Web View: `https://localhost:7xxx/p/{id}`

## Persistence Layer

**PostgreSQL** with **Entity Framework Core 8.0**

### Why PostgreSQL?
- **ACID Compliance**: Ensures data consistency for paste creation and view counting
- **Concurrency Support**: Handles simultaneous paste creation and view tracking reliably
- **JSON Support**: Easily extensible if additional metadata needs to be stored
- **Production Ready**: Robust, scalable, and well-supported in cloud environments
- **Indexing**: Efficient lookups by paste ID (primary key)

### Database Schema
The application uses a single `Pastes` table with the following structure:
- `Id` (string, PK): Unique identifier (GUID without hyphens)
- `Content` (text): The paste content
- `CreatedAt` (timestamp): When the paste was created
- `TtlSeconds` (int?, nullable): Time-to-live in seconds
- `MaxViews` (int?, nullable): Maximum allowed views
- `Views` (int): Current view count

### Migration Management
Migrations are stored in the `Migrations/` folder and can be managed using:
```bash
# Create new migration
dotnet ef migrations add <MigrationName>

# Apply migrations
dotnet ef database update

# Rollback to specific migration
dotnet ef database update <MigrationName>
```

## Important Design Decisions

### 1. **Repository Pattern**
- Abstraction layer (`IPasteRepository`) separates data access from business logic
- Enables easy swapping of persistence implementations (e.g., moving to Redis or in-memory cache)
- Simplifies testing with mock repositories

### 2. **GUID-based IDs**
- Using 32-character hexadecimal GUIDs (`Guid.NewGuid().ToString("N")`)
- Avoids sequential ID enumeration attacks
- URL-safe and collision-resistant
- No need for distributed ID generation coordination

### 3. **View Counting Strategy**
- Views are incremented **after** validation checks (TTL, max views)
- Separate `IncrementViewsAsync()` method ensures atomic updates
- Read-then-increment approach balances consistency with simplicity

### 4. **TTL & Expiration Handling**
- **Lazy expiration**: Expired pastes remain in DB but return 404 on access
- Reduces background job complexity
- Optional: Could add periodic cleanup job for disk space management
- Uses `AppTimeProvider` to support test scenarios with custom timestamps

### 5. **MVC + API Hybrid**
- `PastesApiController`: JSON API for programmatic access (`/api/pastes`)
- `PasteViewController`: Human-friendly HTML rendering (`/p/{id}`)
- Single codebase serves both web and API clients

### 6. **Nullable Optional Fields**
- `TtlSeconds` and `MaxViews` are nullable
- `null` means "no limit" (paste lives forever / unlimited views)
- Reduces storage compared to using sentinel values like `-1`

### 7. **Validation Strategy**
- Request validation done at controller level (thin models)
- Simple rules: content required, TTL/MaxViews must be >= 1 if provided
- Returns clear 400 Bad Request errors with descriptive messages

### 8. **Swagger for Development**
- Enabled only in Development environment
- Provides interactive API documentation
- Excluded from production to reduce attack surface

## API Endpoints

### Create Paste
```http
POST /api/pastes
Content-Type: application/json

{
  "content": "Hello, World!",
  "ttl_seconds": 3600,
  "max_views": 10
}
```

### Get Paste
```http
GET /api/pastes/{id}
```

### View Paste (HTML)
```http
GET /p/{id}
```

## Project Structure
```
PastebinLite/
├── Controllers/       # API and MVC controllers
├── Data/             # DbContext and database configuration
├── DTO/              # Data transfer objects (request/response models)
├── Migrations/       # Entity Framework migrations
├── Models/           # Domain entities (Paste)
├── Services/         # Repository implementations
└── Program.cs        # Application entry point
```
