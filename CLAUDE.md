# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```bash
# Build entire solution
dotnet build

# Run the API (serves at https://localhost:7130)
dotnet run --project ToDoTimeManager.WebApi

# Run the Blazor UI (serves at https://localhost:7262)
dotnet run --project ToDoTimeManager.WebUI

# Build CSS assets (run inside ToDoTimeManager.WebUI/)
npm install
gulp build      # one-time build
gulp            # build + watch for changes
```

No test projects exist in this solution.

## Architecture

Four projects in the solution:

- **ToDoTimeManager.WebApi** — ASP.NET Core 8 REST API (auth, todos, time logs, statistics, user management)
- **ToDoTimeManager.WebUI** — Blazor Server interactive frontend (pages, components, HTTP client services)
- **ToDoTimeManager.Shared** — Models, DTOs, enums, and JWT utilities shared across both runtime projects
- **ToDoTimeManager.DataBase** — SQL Server schema only (tables + stored procedures); no runtime code

### Request Flow

```
Blazor Pages/Components
  → WebUI HttpServices (BaseHttpService subclasses)
    → [HTTP over localhost]
      → WebApi Controllers
        → Business Services (Services/Implementations/)
          → Data Controllers (Services/DataControllers/)
            → DbAccessService (Dapper)
              → SQL Server (stored procedures only — no inline SQL)
```

### Authentication Flow

1. `AuthController` issues JWT access tokens + refresh tokens on login.
2. Tokens are stored in `ProtectedLocalStorage` on the browser.
3. `TokenMessageHandler` (WebUI) injects the Bearer token into every outgoing HTTP request.
4. `ToastMessageHandler` (WebUI) intercepts error responses and surfaces them as toast notifications.
5. `CustomAuthStateProvider` manages Blazor auth state and triggers automatic token refresh on 401 responses.
6. JWT settings (key, issuer, lifetimes) live in `WebApi/appsettings.json`.

### Key Structural Patterns

- **All database access** goes through `DbAccessService` (a generic Dapper wrapper). Controllers and services never write raw SQL; they call stored procedures via DataController interfaces.
- **All WebUI HTTP calls** use typed service classes inheriting from `BaseHttpService` in `WebUI/Services/HttpServices/`.
- **Shared contracts** (models, DTOs, enums) live exclusively in the Shared project — never duplicated in WebApi or WebUI.
- **Localization** is built in; string resources are in `WebUI/Localization/Resource.resx` with `uk-UA` and `en-US` variants.
- **Error handling**: `GlobalExceptionHandler` middleware on the API; `ToastMessageHandler` + `ToastsService` on the UI.

### Notable Directories

| Path | Purpose |
|------|---------|
| `WebApi/Controllers/` | REST endpoints |
| `WebApi/Services/Implementations/` | Business logic |
| `WebApi/Services/DataControllers/` | DB access layer |
| `WebApi/Entities/` | Database-mapped entity types (separate from Shared models) |
| `WebUI/Pages/` | Routed Blazor pages |
| `WebUI/Components/Modals/` | Modal dialog components |
| `WebUI/Components/Toast/` | Toast notification components |
| `WebUI/Handlers/` | HTTP message handlers |
| `WebUI/Services/HttpServices/` | Typed API client services |
| `Shared/Models/` | Domain models (User, ToDo, TimeLog) |
| `Shared/DTOs/` | Request/response DTOs |
| `Shared/Enums/` | ToDoStatus, UserRole, TimeFilter |
| `DataBase/StoredProcedures/` | All SQL stored procedures |
