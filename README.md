# LexiMonSystem

A layered ASP.NET Core 8 Web API for a gamified learning platform. It uses Entity Framework Core with SQL Server, ASP.NET Identity, and JWT authentication.

- Entry point: [`LexiMon.API.Program`](LexiMon.API/Program.cs)
- DbContext: [`LexiMon.Repository.Context.LexiMonDbContext`](LexiMon.Repository/Context/LexiMonDbContext.cs)
- Initial migration: [LexiMon.Repository/Migrations/20250926141043_Initial.cs](LexiMon.Repository/Migrations/20250926141043_Initial.cs)
- API example controller: [LexiMon.API/Controllers/AuthsController.cs](LexiMon.API/Controllers/AuthsController.cs)

## Tech stack

- .NET 8, ASP.NET Core Web API
- EF Core 8 + SQL Server
- ASP.NET Identity
- JWT authentication
- Swagger/OpenAPI
- Docker support

## Solution structure

- LexiMon.API: API host, DI, authentication, Swagger. See [`LexiMon.API.Program`](LexiMon.API/Program.cs).
- LexiMon.Service: Business services and DTOs (e.g. [`LexiMon.Service.Interfaces.IUserService`](LexiMon.Service/Interfaces/IUserService.cs)).
- LexiMon.Repository: EF Core domain models, configurations, migrations, repositories (e.g. [`LexiMon.Repository.Context.LexiMonDbContext`](LexiMon.Repository/Context/LexiMonDbContext.cs)).

## Configuration

Provide the following settings (appsettings.Development.json or environment variables):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=LexiMonDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "SecretKey": "your-very-strong-secret",
    "AccessTokenExpirationSeconds": "1800"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
}
```

Notes:
- JWT validation is configured in [`LexiMon.API.Program`](LexiMon.API/Program.cs) with Bearer scheme.
- On startup, automatic database migration is executed via `db.Database.Migrate()`.

## Run locally

1. Install prerequisites:
    - .NET SDK 8.0
    - SQL Server (local or container)

2. Restore, build, run:
   ```sh
   dotnet restore
   dotnet build
   dotnet run --project LexiMon.API
   ```

3. Open Swagger UI at:
    - http://localhost:5080/swagger (port as configured)

To authorize in Swagger, click Authorize and paste the JWT token.

## Database and migrations

EF Core migrations live in the Repository project. Typical commands:

```sh
# Install EF tools (once)
dotnet tool install --global dotnet-ef

# Add a migration
dotnet ef migrations add <Name> -p LexiMon.Repository -s LexiMon.API

# Apply migrations
dotnet ef database update -p LexiMon.Repository -s LexiMon.API
```

At runtime, migrations are applied automatically on application start.

## Docker

Build and run with Docker Compose:

```sh
docker compose up -d
```

The API container uses the multi-stage Dockerfile at [LexiMon.API/Dockerfile](LexiMon.API/Dockerfile).

## Seeded data

Roles (seeded):
- Free
- Premium
- Admin

Users (seeded in [`LexiMon.Repository.Context.LexiMonDbContext`](LexiMon.Repository/Context/LexiMonDbContext.cs)):
- free@example.com / 12345aA@
- premium@example.com / 12345aA@
- admin@example.com / 12345aA@

Role assignments are seeded; see:
- User role seeding in [`LexiMon.Repository.Context.LexiMonDbContext.SeedingUserRoles`](LexiMon.Repository/Context/LexiMonDbContext.cs)
- Migration seed in [Initial migration](LexiMon.Repository/Migrations/20250926141043_Initial.cs)

## Swagger and security

Swagger is enabled with Bearer security scheme in [`LexiMon.API.Program`](LexiMon.API/Program.cs). Obtain a token via the authentication endpoints and authorize in Swagger to access protected APIs.

## License

MIT License