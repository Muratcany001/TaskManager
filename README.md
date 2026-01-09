# TaskManager

A modular .NET 8 Task/Document Management API. The solution is split into UI (Web API), BLL (business logic), DL (data access), DTOs and Common projects. It supports JWT-based authentication, versioned tasks, document uploads with Google Drive integration, and EF Core for SQL Server persistence.

<img src="https://raw.githubusercontent.com/Muratcany001/TaskManager/main/assets/logo-placeholder.png" alt="TaskManager logo">

> Quick: The API project to run is TM.UI. Swagger UI is available in Development mode.

## Table of contents
- [Architecture](#architecture)
- [Requirements](#requirements)
- [Quick start — run locally](#quick-start---run-locally)
- [Run with Docker](#run-with-docker)
- [Configuration](#configuration)
- [Database & Migrations](#database--migrations)
- [API Reference (selected endpoints)](#api-reference-selected-endpoints)
- [Google Drive integration](#google-drive-integration)
- [Security & secrets](#security--secrets)
- [Troubleshooting & tips](#troubleshooting--tips)
- [Contributing](#contributing)
- [License](#license)

---

## Architecture

Projects:
- TM.UI — ASP.NET Core Web API (entry point). Registers services, EF DbContext, CORS, FluentValidation, AutoMapper, and Swagger.
- TM.BLL — Business logic layer (services, helpers, mappings).
- TM.DL — Data layer with Entity Framework Core DbContext and repositories.
- Dtos — Request/response DTO classes shared between layers.
- Common — Shared utilities/view models.

Key points:
- Uses EF Core (SQL Server). The DbContext and relationships live in TM.DL/Context.cs.
- Authentication uses JWT; tokens are created in TM.BLL.Services.AuthService and persisted to Tokens table.
- Google Drive integration via a service account (credentials.json included — rotate/remove before publishing).

---

## Requirements

- .NET SDK 8.0 (net8.0)
- SQL Server (local or remote) accessible from the app
- (Optional) Docker Desktop (Windows mode required for the provided Dockerfiles)
- dotnet-ef tool (for migrations): install globally if needed
  - dotnet tool install --global dotnet-ef

---

## Quick start — run locally

1. Clone the repository
   ```
   git clone https://github.com/Muratcany001/TaskManager.git
   cd TaskManager
   ```

2. Update configuration
   - Edit `TM.UI/appsettings.json` and set a working SQL Server connection string (see [Configuration](#configuration)).

3. Build the solution
   ```
   dotnet build TaskManager.sln
   ```

4. Apply database migrations (see [Database & Migrations](#database--migrations) if needed)
   ```
   dotnet ef database update --project TM.DL --startup-project TM.UI
   ```

5. Run the API
   ```
   dotnet run --project TM.UI
   ```
   - When running in Development mode, Swagger is available at: `https://localhost:{port}/swagger` (Program.cs enables it in Development).

Notes:
- TM.UI/Program.cs configures CORS to allow localhost:4200 (angular dev server) — adjust if needed.
- The project uses JSON serialization with cycle handling and indented output.

---

## Run with Docker

There are Dockerfiles in `TM.UI/Dockerfile` and `TaskManager/Dockerfile`. Both are based on Windows Nano Server images.

Important: The Dockerfiles use Windows base images (mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809). To build these images you must run Docker in Windows container mode on a host that supports Windows containers. If you need Linux containers, replace the base images with Linux variants (`mcr.microsoft.com/dotnet/aspnet:8.0` / `mcr.microsoft.com/dotnet/sdk:8.0`) and adjust as necessary.

Build and run the TM.UI image (example):
```
# build (from repo root)
docker build -f TM.UI/Dockerfile -t taskmanager-ui:latest .

# run (map container port 8080 to host 8080)
docker run --rm -p 8080:8080 --name taskmanager-ui taskmanager-ui:latest
```

When running in Docker you still need to provide valid configuration (connection string, JWT secret, Google credentials). Use environment variables or mount volumes to inject `appsettings.json` / `credentials.json`.

---

## Configuration

Main configuration file: `TM.UI/appsettings.json`

Example keys you should update:
- ConnectionStrings:DefaultConnection — set to your SQL Server instance.
- Jwt (Key, Issuer, Audience) — set a strong secret for production.

Recommended approaches for secrets:
- Use Environment Variables
- Use ASP.NET Core User Secrets for local development
- Store production secrets in a secure secret store (Azure Key Vault, AWS Secrets Manager, etc.)

Example environment variable override (PowerShell):
```powershell
$env:ConnectionStrings__DefaultConnection = "Server=.;Database=TaskManagerDb;Trusted_Connection=True;TrustServerCertificate=True;"
$env:Jwt__Key = "supersecure-production-key-please-change"
dotnet run --project TM.UI
```

---

## Database & Migrations

The DbContext is in `TM.DL/Context.cs`. The repo includes EF migrations under `TM.DL/Migrations`.

To add a migration (from repo root):
```
dotnet ef migrations add MigrationName --project TM.DL --startup-project TM.UI
```

To apply migrations / update the database:
```
dotnet ef database update --project TM.DL --startup-project TM.UI
```

Notes:
- TM.DL contains a `ContextFactory` (design-time) that sets a default connection string — replace it or prefer to pass connection via `--startup-project` configuration to avoid hard-coded credentials.
- Ensure SQL Server is reachable and the connection string is valid.

---

## API Reference (selected endpoints)

Base URL when running locally is typically `https://localhost:{port}`.

The following endpoints are representative (see controllers for a full list):

1) Create user (Registration)
- POST /users/CreateUser
- Body (JSON):
  ```json
  {
    "name": "Alice Example",
    "email": "alice@example.com",
    "password": "plaintext-password"
  }
  ```
- Response: 200 OK with created user result or 400 on validation errors.

cURL:
```
curl -X POST https://localhost:5001/users/CreateUser \
  -H "Content-Type: application/json" \
  -d '{"name":"Alice","email":"alice@example.com","password":"pass123"}' -k
```

2) Login
- POST /users/Login
- Body:
  ```json
  {
    "email": "alice@example.com",
    "password": "pass123"
  }
  ```
- Response: 200 OK { Message: "Giriş başarılı", UserId: <id> } OR Unauthorized on failure.
- Note: Under the hood a JWT is generated and stored.

cURL:
```
curl -X POST https://localhost:5001/users/Login \
  -H "Content-Type: application/json" \
  -d '{"email":"alice@example.com","password":"pass123"}' -k
```

3) Create a Task Version
- POST /version/GetNewVersion
- Body (CreateVersionDto – fields may include taskId, status, CreatedByUserId etc.)
- Response: 201 Created with version info or 400 on validation failure.

4) Get Latest Version by Task
- GET /version/GetLatestVersion/{taskId}
- Response: 200 OK with version DTO or 400/404.

5) Document upload (example uses multipart/form-data)
- POST /Document/CreateDocument/{taskId}/{title}
- Form parameters:
  - file: file to upload (multipart)
  - other DTO fields in form-data as required
- cURL example (replace {taskId} and {title}):
```
curl -X POST "https://localhost:5001/Document/CreateDocument/123/ExampleTitle" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@/path/to/file.pdf" -k
```
- Response: created document details or 400/404 on errors.

6) Delete Document
- DELETE /Document/DeleteCoument/{documentId}
- Response: 200 OK "Belge silindi" or 400 if not found.

Notes:
- All routes in controllers are attribute-routed (check `TM.UI/Controllers/*` for exact parameter names and validation).
- Swagger (in Development) provides interactive docs: `https://localhost:{port}/swagger`

---

## Google Drive integration

- A `credentials.json` service-account file is included at `TM.UI/credentials.json`. This file contains a private key for a Google service account and is sensitive.
- The project uses Google Drive APIs (see TM.BLL references to Google.Apis.Drive.v3). The GoogleDriveService integrates file uploads to Drive.
- To use Drive integration:
  - Create a Google Cloud service account with Drive API enabled.
  - Download the JSON key file and place it at `TM.UI/credentials.json` (or supply via secure mount/secret).
  - Make sure the service account has access to the desired Drive or shared folder.

Important: Never commit real service-account keys to public repos. Rotate/replace the key in this repository immediately if it was committed accidentally.

---

## Security & secrets

- There is a private key inside `TM.UI/credentials.json` in this repo — treat it as compromised.
  - Action: Remove the file from the repo and history (use git filter-branch / BFG) and create a new service-account key.
- appsettings.json contains `Jwt:Key` and a connection string pointing to a developer machine. Replace these with secure values.
- Recommended:
  - Use environment variables or a secret store for production credentials.
  - Use ASP.NET Core Data Protection and secure cookie settings if you extend authentication.
  - Use HTTPS (the default ASP.NET Core template does).

---

## Troubleshooting & tips

- If EF migrations complain about the DbContext or connection:
  - Ensure you run the `dotnet ef` commands from a shell where the .NET SDK 8.0 is active.
  - Use `--project TM.DL --startup-project TM.UI` flags to target the right projects.
- Docker Windows container issues:
  - Switch Docker Desktop to "Windows containers" mode to build the provided Dockerfiles, or adapt the Dockerfile to Linux base images for Linux containers.
- CORS:
  - TM.UI config allows origins `https://localhost:4200` and `http://localhost:4200`. Change CORS policy in `Program.cs` if your frontend runs on a different origin.
- Logging:
  - Logging levels are set in `TM.UI/appsettings.json`. Adjust `LogLevel` for more/less verbosity.

---

## Contributing

Contributions are welcome. Suggested workflow:
- Fork the repo and create a feature branch.
- Keep secrets out of commits.
- Add tests where applicable and ensure the project builds:
  ```
  dotnet build TaskManager.sln
  ```
- Open a pull request with a clear description of changes.

---

## License

This repository does not include a license file. Add a LICENSE to clarify terms. For personal or open-source projects, consider MIT, Apache-2.0, or similar.

---

If you want, I can:
- Generate a sample `.env` and example `appsettings.Production.json` with environment-variable overrides.
- Draft a small Postman/HTTP collection from the controllers for quick testing.
- Suggest a minimal change to the Dockerfiles to support Linux containers.
