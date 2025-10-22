# JWT (ASP.NET Core Web API)

Short description
This is an ASP.NET Core Web API (targeting .NET 8) that demonstrates JWT-based authentication with refresh tokens using ASP.NET Core Identity and Entity Framework Core (SQL Server). It includes user registration, login, refresh-token rotation, role management, and a protected sample endpoint. Swagger (OpenAPI) is included for interactive testing.

Key features
- ASP.NET Core Web API (.NET 8)
- Authentication with JWT access tokens and refresh tokens
- ASP.NET Core Identity backed by Entity Framework Core (SQL Server)
- Role management (create & assign roles)
- Protected example endpoint (`WeatherForecast`) requiring authorization
- Swagger UI for endpoint exploration
- EF Core migrations support

Project layout (high level)
- `Program.cs` — registers services: Identity, EF Core (SQL Server), JWT auth and Swagger.
- `Controllers/AuthController.cs` — endpoints:
  - `POST /api/Auth/register` — register user
  - `POST /api/Auth/login` — login and receive `AccessToken` + `RefreshToken`
  - `POST /api/Auth/refreshToken` — exchange refresh token for new tokens
  - `POST /api/Auth/addRole` — create role (Admin-only)
  - `POST /api/Auth/assignRole` — assign role to user (Admin-only)
  - `GET /api/Auth/getUsers` — list users
- `Controllers/WeatherForecastController.cs` — protected sample endpoint (`[Authorize]`)
- `Dtos/` — DTOs for login/register/refresh operations
- `Data/Context` — EF Core DbContext + migrations folder
- `Services/IAuthService` + implementation — token generation/refresh logic (used by controller)
- `JWT.csproj` — project file with NuGet package dependencies

Dependencies (from project file)
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Proxies
- Microsoft.EntityFrameworkCore.Tools
- Swashbuckle.AspNetCore

Configuration (what to set)
Add or update `appsettings.json` with:
- `ConnectionStrings:defaultConnection` — your SQL Server connection string
- `JWT:ValidAudience` — audience value used to validate tokens
- `JWT:ValidIssuer` — issuer value used to validate tokens
- `JWT:SecretKey` — strong symmetric key for signing tokens (store securely; do NOT commit secrets)

Example minimal `appsettings.json` snippet:
