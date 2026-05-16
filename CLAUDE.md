# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```bash
# Run with Aspire (recommended — starts DbPublisher → WebApi → WebUI automatically)
dotnet run --project ToDoTimeManager.AppHost

# Or run individually (manual order matters):
dotnet run --project ToDoTimeManager.WebApi   # serves at https://localhost:7130
dotnet run --project ToDoTimeManager.WebUI    # serves at https://localhost:7262

# Build CSS assets (run inside ToDoTimeManager.WebUI/)
npm install
gulp build      # one-time build
gulp            # build + watch for changes
```

No test projects exist in this solution.

---

## Solution Structure

The solution uses **.NET 10** and is organized into **14 projects** grouped by solution folder.

### Backend

| Project | Purpose |
|---------|---------|
| `ToDoTimeManager.Entities` | Dapper-mapped entity types + typed exception hierarchy |
| `ToDoTimeManager.DataAccess` | DataController interfaces & implementations + generic `DbAccessService` (Dapper) |
| `ToDoTimeManager.Business` | Business service interfaces & implementations |
| `ToDoTimeManager.Business.Utils` | Utility service interfaces & implementations (JWT, passwords, email, 2FA, hashing) |
| `ToDoTimeManager.WebApi` | ASP.NET Core 10 REST API; all controllers inherit `BaseController` |

### Aspire

| Project | Purpose |
|---------|---------|
| `ToDoTimeManager.AppHost` | .NET Aspire orchestrator — `DbPublisher` completes first, then `WebApi` starts, then `WebUI` |
| `ToDoTimeManager.DbPublisher` | Runs on startup to deploy the SQL Server schema and apply migrations |
| `ToDoTimeManager.ServiceDefaults` | Shared Aspire configuration (health checks, OpenTelemetry, service discovery) |

### Frontend

| Project | Purpose |
|---------|---------|
| `ToDoTimeManager.WebUI` | Blazor Server app shell — `Program.cs`, `App.razor`, Pages, Handlers, `CultureController` |
| `ToDoTimeManager.WebUI.Components` | All Razor components (base, page, modal, static, shared/layouts) |
| `ToDoTimeManager.WebUI.Services` | HTTP services, auth state, modal/toast services, circuit accessor |
| `ToDoTimeManager.WebUI.Models` | UI-only enums, models, interfaces |
| `ToDoTimeManager.WebUI.Utils` | Protected-storage helpers, string validation, culture helper, page-title helper |

### Database

| Project | Purpose |
|---------|---------|
| `ToDoTimeManager.DataBase` | SQL Server `sqlproj` — tables + stored procedures + migrations; no runtime code |

### Shared

| Project | Purpose |
|---------|---------|
| `ToDoTimeManager.Shared` | Models, DTOs, enums, extensions, and JWT utilities shared across all runtime projects |

---

## Request Flow

```
Blazor Pages (WebUI)
  → HttpServices (WebUI.Services/HttpServices)
    → [HTTP over Aspire service discovery / localhost]
      → WebApi Controllers (inherit BaseController)
        → Business Services (Business/Services)
          → DataAccess Controllers (DataAccess/DataControllers)
            → DbAccessService (DataAccess/DbAccessServices, Dapper)
              → SQL Server (stored procedures only — no inline SQL)
```

---

## Authentication Flow

1. `AuthController.Login` validates credentials, sends a 2FA code to the user's email via `IEmailService`, and returns a `TwoFactorPendingModel` (userId + masked email). A JWT is **not** issued yet.
2. `AuthController.SendCode` resends a code (rate-limited separately).
3. `AuthController.VerifyCode` validates the code and issues a `TokenModel` (access token + refresh token).
4. Tokens are stored in `ProtectedLocalStorage` via `TokenProtectedStorageHelper`.
5. `TokenMessageHandler` attaches the Bearer token to every outgoing HTTP request and transparently retries on 401 using `TokenRefreshService`.
6. `CustomAuthStateProvider` manages Blazor auth state and also performs refresh when the access token is expired but the refresh token is still valid.
7. JWT settings live in `WebApi/appsettings.json`. ClockSkew is `TimeSpan.Zero`.
8. Auth endpoints are rate-limited: `auth-login`, `auth-send-code`, `auth-verify-code`, `auth-register`.

---

## Authorization Model

Five roles are defined in `Shared/Enums/UserRole.cs` as a numeric hierarchy:

| Role | Value | Notes |
|------|-------|-------|
| `User` | 0 | Default on registration |
| `Developer` | 1 | |
| `ProjectManager` | 2 | |
| `Manager` | 3 | |
| `Admin` | 4 | Unrestricted access |

`BaseController` exposes hierarchy helpers: `IsAdmin()`, `IsManager()`, `IsProjectManager()`, `IsDeveloper()` — each returns `true` for that role **and all higher roles**.

`TeamMemberRole` (Member=0, Owner=1) governs per-team permissions for team and project operations.

Resource ownership checks (is this the caller's own resource?) are performed inside **business services**, not controllers.

---

## Key Structural Patterns

- **All database access** goes through `DbAccessService` (a generic Dapper wrapper). No inline SQL anywhere — stored procedures only.
- **All WebUI HTTP calls** use typed service classes inheriting from `BaseHttpService` (`WebUI.Services/HttpServices/`). The named `HttpClient` is `"TodoTimeManager"`.
- **Shared contracts** (models, DTOs, enums, extensions) live exclusively in `ToDoTimeManager.Shared` — never duplicated in any other project.
- **Entities** (`Entities/Entities/`) are Dapper-mapped types separate from shared models. They are used only in the DataAccess and Business layers — never leak to WebUI.
- **Exception hierarchy** — business/data code throws typed exceptions from `Entities/Exceptions/`; `GlobalExceptionHandler` middleware maps them to HTTP status codes via `ProblemDetailsFactory`:

  | Exception | HTTP Status |
  |-----------|------------|
  | `ValidationException` | 400 |
  | `ForbiddenException` | 403 |
  | `NotFoundException` | 404 |
  | `ConflictException` | 409 |
  | `ServiceException` (base) | set in constructor |

- **Activity log tracking** — mutating operations on ToDos, TimeLogs, and Users must write entries via `IActivityLogsService`.
- **Localization** — resource strings live in `WebUI.Components/Resources/` (`Resource.resx`, `Resource.uk-UA.resx`, `Resource.en-US.resx`). Default culture is `uk-UA`. Inject `IStringLocalizer<Resource>` in components (already provided by `BaseComponent`).
- **CircuitServicesAccesor** — provides `IServiceProvider` access from outside the Blazor circuit scope (used by `CustomAuthStateProvider` and `TokenMessageHandler`).
- **Data seeding** — `WebApi/Seeders/DataSeeder.cs` creates a seed admin user on startup if one does not exist.

---

## Notable Directories

### Backend

| Path | Purpose |
|------|---------|
| `ToDoTimeManager.Entities/Entities/` | Dapper-mapped entity types (one per DB table) |
| `ToDoTimeManager.Entities/Exceptions/` | `ServiceException`, `ConflictException`, `ForbiddenException`, `NotFoundException`, `ValidationException` |
| `ToDoTimeManager.DataAccess/DataControllers/Interfaces/` | Data controller interfaces (one per entity) |
| `ToDoTimeManager.DataAccess/DataControllers/Implementation/` | Data controller implementations |
| `ToDoTimeManager.DataAccess/DbAccessServices/DbAccessService.cs` | Generic Dapper wrapper |
| `ToDoTimeManager.Business/Services/Interfaces/` | Business service interfaces |
| `ToDoTimeManager.Business/Services/Implementations/` | Business service implementations |
| `ToDoTimeManager.Business.Utils/Interfaces/` | `IJwtGeneratorService`, `IPasswordHelperService`, `ITwoFactorCodesHelper`, `IEmailService` |
| `ToDoTimeManager.Business.Utils/Implementations/` | `JwtGeneratorService`, `PasswordHelperService`, `TwoFactorCodesHelper`, `EmailService`, `HashHelper` |
| `ToDoTimeManager.WebApi/Controllers/BaseController.cs` | Abstract base for all controllers |
| `ToDoTimeManager.WebApi/Controllers/` | REST endpoint controllers |
| `ToDoTimeManager.WebApi/Middleware/GlobalExceptionHandler.cs` | Catches `ServiceException` subclasses and maps to ProblemDetails |
| `ToDoTimeManager.WebApi/Extensions/ProblemDetailsFactory.cs` | Builds `ProblemDetails` responses by status code |
| `ToDoTimeManager.WebApi/Extensions/StringExtensions.cs` | String utility extensions |
| `ToDoTimeManager.WebApi/Seeders/DataSeeder.cs` | Seeds the initial admin user on startup |

### Frontend

| Path | Purpose |
|------|---------|
| `ToDoTimeManager.WebUI/Pages/` | Routed Blazor pages (`MainPage.razor`, `AuthPage.razor`) |
| `ToDoTimeManager.WebUI/Handlers/` | `TokenMessageHandler`, `ToastMessageHandler` |
| `ToDoTimeManager.WebUI/Controllers/CultureController.cs` | MVC controller for culture switching |
| `ToDoTimeManager.WebUI.Components/BaseComponents/BaseComponent.cs` | Abstract base for all Razor components |
| `ToDoTimeManager.WebUI.Components/PageComponents/AuthPage/` | `BaseAuthForm`, `StepsComponent`, `Forms/` (LoginForm, RegisterForm, TwoFAForm) |
| `ToDoTimeManager.WebUI.Components/StaticComponents/Elements/` | `Input`, `Button`, `CheckInput`, `CustomDropdown`, `CultureSelector` |
| `ToDoTimeManager.WebUI.Components/StaticComponents/Icons/` | All SVG icon components as Razor files + `Countries/` flags |
| `ToDoTimeManager.WebUI.Components/StaticComponents/Toast/` | `Toast.razor`, `ToastBox.razor` |
| `ToDoTimeManager.WebUI.Components/Modals/` | `ModalContainer`, `ConfirmModal` |
| `ToDoTimeManager.WebUI.Components/Shared/` | `NavMenu`, `Scene` |
| `ToDoTimeManager.WebUI.Components/Shared/Layouts/` | `MainLayout`, `AuthLayout`, `Helpers/RedirectToLogin` |
| `ToDoTimeManager.WebUI.Components/Resources/` | `Resource.resx`, `Resource.uk-UA.resx`, `Resource.en-US.resx` |
| `ToDoTimeManager.WebUI.Services/HttpServices/` | `BaseHttpService` + typed API client services |
| `ToDoTimeManager.WebUI.Services/Services/Implementations/` | `CustomAuthStateProvider`, `ToastsService`, `ModalService`, `TokenRefreshService`, `TwoFaTimerService` |
| `ToDoTimeManager.WebUI.Services/Services/Interfaces/` | `IToastsService`, `IModalService`, `ITwoFaTimerService` |
| `ToDoTimeManager.WebUI.Services/Helpers/CircuitServicesAccesor/` | `CircuitServicesAccesor.cs`, `ServicesAccessorCuircutHandler.cs` |
| `ToDoTimeManager.WebUI.Services/Helpers/Modal/` | `ModalParameters.cs`, `ModalReference.cs` |
| `ToDoTimeManager.WebUI.Models/Enums/` | `AuthPageCurrentState`, `ToastType`, button/input style enums |
| `ToDoTimeManager.WebUI.Models/Models/` | `ToastModel`, `PendingTwoFaSessionState` |
| `ToDoTimeManager.WebUI.Utils/PotectedLocalStorageHelpers/` | `TokenProtectedStorageHelper`, `ProtectedStorageHelper` |
| `ToDoTimeManager.WebUI.Utils/StringHelpers/` | `StringValidationHelper` |
| `ToDoTimeManager.WebUI.Utils/OtherUtils/` | `CultureInfoHelper`, `ProblemDetailsParser` |
| `ToDoTimeManager.WebUI.Utils/PagesHelpers/` | `PageTitleHelper` |

### Shared

| Path | Purpose |
|------|---------|
| `ToDoTimeManager.Shared/Models/` | Domain models (`User`, `ToDo`, `TimeLog`, `Team`, `Project`, `ActivityLog`, …) |
| `ToDoTimeManager.Shared/DTOs/` | Request/response DTOs, grouped by entity subfolder |
| `ToDoTimeManager.Shared/Enums/` | All shared enums |
| `ToDoTimeManager.Shared/Extensions/` | `TimeFilterExtensions`, `MappingExtensions` |
| `ToDoTimeManager.Shared/Utils/` | `JwtTokenHelper`, `NotEmptyGuidAttribute` |

### Database

| Path | Purpose |
|------|---------|
| `ToDoTimeManager.DataBase/Tables/` | SQL table creation scripts |
| `ToDoTimeManager.DataBase/StoredProcedures/` | All stored procedures, grouped by entity subfolder |
| `ToDoTimeManager.DataBase/Migrations/` | Versioned migration scripts applied by `DbPublisher` at startup |

---

## API Endpoints

All controllers are routed under `api/[Controller]`.

### BaseController

All controllers inherit `BaseController`, which provides:
- `GetCurrentUserId()` — extracts `UserId` from JWT claims
- `GetCurrentUserRole()` — extracts `UserRole` from JWT claims
- `IsAdmin()`, `IsManager()`, `IsProjectManager()`, `IsDeveloper()` — returns `true` for that role and all higher roles

### AuthController — `[AllowAnonymous]`

| Verb | Route | Description |
|------|-------|-------------|
| POST | `Auth/Login` | Validate credentials; send 2FA code to email; return `TwoFactorPendingModel` |
| POST | `Auth/SendCode` | Resend 2FA code |
| POST | `Auth/VerifyCode` | Verify 2FA code; return `TokenModel` |
| POST | `Auth/RefreshToken` | Exchange refresh token for new access token |

### UsersController

| Verb | Route | Authorization | Description |
|------|-------|---------------|-------------|
| GET | `Users/GetAll` | Manager, Admin | Get all users (returns `UserResponseDto` — no passwords) |
| GET | `Users/GetById/{id}` | Auth | Get user by ID (admin or self) |
| GET | `Users/GetByUsername/{userName}` | Auth | Get user by username |
| GET | `Users/GetByEmail/{email}` | Auth | Get user by email |
| GET | `Users/GetByLoginParameter/{loginParameter}` | Auth | Get user by username or email |
| POST | `Users/Create` | AllowAnonymous | Register new user |
| PUT | `Users/Update` | Auth | Update own profile |
| PUT | `Users/ChangeRole/{id}` | Admin | Change user's role |
| DELETE | `Users/Delete/{id}` | Admin | Delete user |

### ToDosController — `[Authorize]`

| Verb | Route | Authorization | Description |
|------|-------|---------------|-------------|
| GET | `ToDos/GetAll` | Manager, Admin | Get all to-dos |
| GET | `ToDos/GetById/{id}` | Auth | Get to-do (admin or assignee) |
| GET | `ToDos/GetByUserId/{userId}` | Auth | Get user's to-dos (admin or self) |
| POST | `ToDos/Create` | Auth | Create new to-do |
| PUT | `ToDos/Update` | Auth | Update to-do (admin or assignee) |
| DELETE | `ToDos/Delete/{id}` | Auth | Delete to-do (admin or assignee) |

### TimeLogsController — `[Authorize]`

| Verb | Route | Authorization | Description |
|------|-------|---------------|-------------|
| GET | `TimeLogs/GetAll` | Manager, Admin | Get all time logs |
| GET | `TimeLogs/GetById/{id}` | Auth | Get log (admin or owner) |
| GET | `TimeLogs/GetByToDoId/{toDoId}` | Auth | Get logs for a to-do |
| GET | `TimeLogs/GetByUserId/{userId}` | Auth | Get user's logs (admin or self) |
| GET | `TimeLogs/GetByUserIdAndToDoId/{userId}/{toDoId}` | Auth | Get logs for user + to-do |
| POST | `TimeLogs/Create` | Auth | Create time log entry |
| PUT | `TimeLogs/Update` | Auth | Update log (admin or owner) |
| DELETE | `TimeLogs/Delete/{id}` | Auth | Delete log (admin or owner) |

### TeamsController — `[Authorize]`

| Verb | Route | Authorization | Description |
|------|-------|---------------|-------------|
| GET | `Teams/GetAll` | Manager, Admin | Get all teams |
| GET | `Teams/GetById/{id}` | Auth | Get team (admin or member) |
| GET | `Teams/GetToDosByTeamId/{teamId}` | Auth | Get team's to-dos |
| GET | `Teams/GetMyTeams` | Auth | Get current user's teams |
| PUT | `Teams/Update` | Auth | Update team (admin or creator/owner) |
| POST | `Teams/AddMember` | Auth | Add member to team |
| DELETE | `Teams/RemoveMember/{teamId}/{userId}` | Auth | Remove member from team |
| DELETE | `Teams/Delete/{id}` | Admin | Delete team |

### ProjectsController — `[Authorize]`

| Verb | Route | Authorization | Description |
|------|-------|---------------|-------------|
| GET | `Projects/GetAll` | Admin | Get all projects |
| GET | `Projects/GetById/{id}` | Auth | Get project (admin or member via team) |
| GET | `Projects/GetToDosByProjectId/{projectId}` | Auth | Get project's to-dos |
| GET | `Projects/GetMyProjects` | Auth | Get current user's accessible projects |
| PUT | `Projects/Update` | Auth | Update project (admin or creator) |
| POST | `Projects/AddTeam` | Auth | Associate team with project |
| DELETE | `Projects/RemoveTeam/{projectId}/{teamId}` | Auth | Remove team from project |
| DELETE | `Projects/Delete/{id}` | Admin | Delete project |

### StatisticController — `[Authorize]`

| Verb | Route | Authorization | Description |
|------|-------|---------------|-------------|
| GET | `Statistic/GetToDoCountStatisticsOfAllTimeByUserId/{userId}` | Auth | To-do counts by status (admin or self) |
| POST | `Statistic/GetMainPageStatistic` | Auth | Dashboard stats (time logs, upcoming due dates, status counts) |

### ActivityLogsController — `[Authorize]`

| Verb | Route | Authorization | Description |
|------|-------|---------------|-------------|
| GET | `ActivityLogs/GetAll` | Manager, Admin | Get all activity logs |
| GET | `ActivityLogs/GetByToDoId/{toDoId}` | Auth | Get logs for a to-do |
| GET | `ActivityLogs/GetByUserId/{userId}` | Auth | Get logs for a user |
| GET | `ActivityLogs/GetByUserIdAndToDoId/{userId}/{toDoId}` | Auth | Get logs for user + to-do |

---

## Shared Project

### Enums (`Shared/Enums/`)

| Enum | Values |
|------|--------|
| `UserRole` | User=0, Developer=1, ProjectManager=2, Manager=3, Admin=4 |
| `ToDoStatus` | New, InProgress, Completed, OnHold, Cancelled |
| `TeamMemberRole` | Member=0, Owner=1 |
| `TimeFilter` | AllTime, DayAgo, WeekAgo, MonthAgo, YearAgo |
| `TaskType` | UserStory=0, Feature=1, Bug=2, Incident=3, Support=4, Meet=5 |
| `ProjectType` | Backend=0, Frontend=1, DataBase=2, FullStack=3, Mobile=4, DevOps=5, DataScience=6, Security=7, Other=8 |
| `ActivityType` | ToDoCreated=0, ToDoUpdated=1, ToDoDeleted=2, StatusChanged=3, TimeLogged=4, TimeLogUpdated=5, TimeLogDeleted=6, UserUpdated=7, UserRoleChanged=8 |

### Models (`Shared/Models/`)

| Model | Key Fields |
|-------|-----------|
| `User` | Id, UserName, Email, Password, UserRole |
| `ToDo` | Id, NumberedId, Title, Description, CreatedAt, DueDate, Status, Type (TaskType?), AssignedTo, TeamId, ProjectId, DisplayDueDate (computed) |
| `TimeLog` | Id, ToDoId, UserId, HoursSpent (decimal), LogDate, LogDescription |
| `Team` | Id, Name, Description, CreatedAt, CreatedBy, MemberCount |
| `Project` | Id, Name, Description, CreatedAt, CreatedBy, TeamCount |
| `TeamMember` | Id, TeamId, UserId, Role |
| `ProjectTeam` | Id, ProjectId, TeamId |
| `ActivityLog` | Id, ToDoId?, UserId, Type (ActivityType), Description, ActivityTime, UserName?, ToDoTitle?, ToDoNumberedId? |
| `TokenModel` | AccessToken, RefreshToken, RefreshTokenExpiresAt |
| `TwoFactorPendingModel` | UserId, MaskedEmail |
| `LoginUser` | LoginParameter (username or email), Password |
| `MainPageStatisticModel` | TimeLogsForGivenTime, TimeLogsForThisMonth, DueDateTasks (Dictionary\<DateTime, ToDo\>), ToDoStatuses |
| `ToDoCountStatisticsOfAllTime` | Statistics by status |

### DTOs (`Shared/DTOs/`)

Organized into subfolders by entity:

- **User/** — `CreateUserRequestDto`, `UpdateUserRequestDto`, `UserResponseDto` (no password), `ChangeUserRoleRequestDto`
- **ToDo/** — `ToDoUpsertRequestDto`
- **TimeLog/** — `TimeLogUpsertRequestDto`
- **Team/** — `CreateTeamRequestDto`, `UpdateTeamRequestDto`, `TeamResponseDto`, `TeamMemberUpsertRequestDto`
- **Project/** — `CreateProjectRequestDto`, `UpdateProjectRequestDto`, `ProjectResponseDto`, `ProjectTeamUpsertRequestDto`
- **MainPageStatistic/** — `MainPageStatisticRequestDto`
- **TwoFactorAuth/** — `SendTwoFactorCodeRequestDto`, `VerifyTwoFactorRequestDto`

### Extensions (`Shared/Extensions/`)

- `TimeFilterExtensions` — converts `TimeFilter` enum to a `DateTime` cutoff.
- `MappingExtensions` — `.ToResponseDto()` and similar mapping helpers.

### Utils (`Shared/Utils/`)

- `JwtTokenHelper` — parses `UserId` and `UserRole` claims from an access token string.
- `NotEmptyGuidAttribute` — `ValidationAttribute` that rejects `Guid.Empty`.

---

## Database

### Tables (`DataBase/Tables/`)

All primary keys are `UNIQUEIDENTIFIER`. All FKs reference other tables by GUID.

| Table | Columns |
|-------|---------|
| `Users` | Id, Username, Email, Password, UserRole (INT) |
| `UsersSecrets` | Id, UserId→Users, PasswordHash, PasswordSalt, RefreshToken, RefreshTokenExpiresAt |
| `ToDos` | Id, NumberedId (INT), Title, Description, CreatedAt, DueDate, Status (INT), Type (INT nullable), AssignedTo→Users, TeamId→Teams, ProjectId→Projects |
| `Teams` | Id, Name, Description, CreatedAt, CreatedBy→Users |
| `TeamMembers` | Id, TeamId→Teams, UserId→Users, Role (INT), UQ(TeamId, UserId) |
| `Projects` | Id, Name, Description, CreatedAt, CreatedBy→Users |
| `ProjectTeams` | Id, ProjectId→Projects, TeamId→Teams, UQ(ProjectId, TeamId) |
| `TimeLogs` | Id, ToDoId→ToDos, UserId→Users, HoursSpent (DECIMAL), LogDate, LogDescription |
| `TwoFactorCodes` | Id, UserId→Users, Code, ExpiresAt |
| `ActivityLogs` | Id, ToDoId→ToDos (nullable), UserId→Users, Type (INT), Description, ActivityTime |

### Stored Procedures (`DataBase/StoredProcedures/`)

Naming convention: `sp_[EntityName]_[Operation]`

| Entity | Procedures |
|--------|-----------|
| Users | GetAll, GetById, GetbyUsername, GetByEmail, GetByLoginParameter, Create, Update, DeleteById |
| ToDos | GetAll, GetById, GetByAssignedTo, GetByTeamId, GetByProjectId, GetByNearestDueDateByUserId, GetCountByUserIdAndStatus, Create, Update, DeleteById |
| TimeLogs | GetAll, GetById, GetByToDoId, GetByUserId, GetByUserIdAndToDoId, GetByUserIdAndTime, Create, Update, DeleteById |
| Teams | GetAll, GetById, GetByUserId, Create, Update, DeleteById |
| TeamMembers | GetById, GetByTeamId, GetByTeamIdAndUserId, Create, DeleteByTeamIdAndUserId |
| Projects | GetAll, GetById, GetByUserId, Create, Update, DeleteById |
| ProjectTeams | GetByProjectId, GetByProjectIdAndTeamId, Create, DeleteByProjectIdAndTeamId |
| TwoFactorCodes | GetByUserId, Upsert, DeleteByUserId |
| ActivityLogs | GetAll, GetByUserId, GetByToDoId, GetByUserIdAndToDoId, Create |
| AccessControl | CanUserAccessProject, CanUserAccessTeam, CanUserAccessTimeLog, CanUserAccessToDo |
| UsersSecrets | (via `IUserSecretsDataController`) |

### Migrations (`DataBase/Migrations/`)

Versioned migration scripts applied by `DbPublisher` at startup. New `.sql` migration files must be registered in `ToDoTimeManager.DataBase.sqlproj`.

---

## Aspire Orchestration

Startup order enforced in `ToDoTimeManager.AppHost/Program.cs`:

```
DbPublisher  (WaitForCompletion — deploys schema + applies migrations)
  → WebApi   (WaitForCompletion(dbPublish))
    → WebUI  (WaitFor(api), WithReference(api))
```

`ToDoTimeManager.ServiceDefaults` provides shared Aspire configuration (health-check endpoints, OpenTelemetry, service-discovery defaults) and is referenced by both `WebApi` and `WebUI`.

When running under Aspire, the WebUI `HttpClient` base address resolves to `https+http://webapi` via service discovery. `BaseApiUrlAddress` in `WebUI/appsettings.json` is only used when running WebUI in isolation.

---

## WebUI Pages

Only two routed pages exist in `ToDoTimeManager.WebUI/Pages/`:

| File | Route | Description |
|------|-------|-------------|
| `MainPage.razor` | `/` | Dashboard page |
| `AuthPage.razor` | `/auth` | Multi-step auth: Login → 2FA, Registration → 2FA |

Both use code-behind files (`.razor.cs`). Layouts: `MainLayout.razor` for authenticated routes, `AuthLayout.razor` for the auth page.

### AuthPage Flow

`AuthPage` manages state via `AuthPageCurrentState` enum (`Login`, `Registration`, `TwoFA`) and drives animated CSS slide transitions between three sub-components:

```
AuthPage.razor
  └── BaseAuthForm (template: icon, title, subtitle, step indicator)
        ├── LoginForm      — credentials entry (step 1 of login)
        ├── RegisterForm   — account creation (step 1 of registration)
        └── TwoFAForm      — 2FA code entry (step 2 of both flows)
```

Animation uses CSS classes (`--active`, `--hidden-right`, `--hidden-left`, `--exiting-*`, `--entering-*`) applied with a 450 ms transition.

---

## WebUI Components (`ToDoTimeManager.WebUI.Components`)

### BaseComponents

- **`BaseComponent.cs`** — Abstract base for all Razor components. Provides:
  - Injected `IStringLocalizer<Resource> Localizer`
  - `IsLoading` flag + `SkeletonLoading` CSS helper string
  - `Loading(Func<Task>)` async wrapper (sets `IsLoading`, calls `StateHasChanged`)
  - `GetPageTitle(string)` returns a `<PageTitle>` `RenderFragment` with localized name + " - TaskForge"

### PageComponents (`PageComponents/AuthPage/`)

- **`BaseAuthForm.razor`** — Template layout for multi-step auth. Parameters: `FormContent` (RenderFragment), `Steps` (List\<string\>), `CurrentStep`, `MainIcon`, `MainText`, `SubText`.
- **`StepsComponent.razor`** — Multi-step progress indicator.
- **`Forms/LoginForm.razor`**, **`RegisterForm.razor`**, **`TwoFAForm.razor`** — Auth step components.

### StaticComponents / Elements

- **`Input.razor`** — Validated text input with icon support. Key parameters: `Value`/`ValueChanged`, `Type`, `IsPassword` (toggleable eye button), `Icon`, `IconPosition` (Left/Right), `UseValidation`, `ValidationFunc`. Pre-built validators in `StringValidationHelper`: `DefaultValidation`, `EmailValidation`, `PasswordValidation`, `UsernameValidation`, `ConfirmPasswordValidation`, `EmailOrUsernameValidation`.
- **`Button.razor`** — Button with icon, style variants (`Primary`, `Secondary`, `Ghost`), and optional `IsLoading` spinner.
- **`CheckInput.razor`** — Checkbox with label.
- **`CustomDropdown.razor`** — Searchable dropdown with single/multi-select and templated items.
- **`CultureSelector.razor`** — Language switcher (en-US / uk-UA), posts to `CultureController`.

### StaticComponents / Icons

All SVG icon components as individual `.razor` files (Bootstrap Icons). Country flag icons are nested under `Icons/Countries/`.

### StaticComponents / Toast

- **`Toast.razor`** — Single toast notification.
- **`ToastBox.razor`** — Container that renders active toasts.

### Modals

- **`ModalContainer.razor`** — Dynamic modal host; renders modals registered via `IModalService`.
- **`ConfirmModal.razor`** — Generic confirmation dialog.

### Shared

- **`NavMenu.razor`** — Navigation sidebar.
- **`Scene.razor`** — Page scene/background wrapper.
- **`Layouts/MainLayout.razor`** — Authenticated layout.
- **`Layouts/AuthLayout.razor`** — Unauthenticated layout.
- **`Layouts/Helpers/RedirectToLogin.razor`** — Redirects unauthenticated users to `/auth`.

---

## WebUI Services (`ToDoTimeManager.WebUI.Services`)

### HTTP Services (`HttpServices/`)

`BaseHttpService` creates the named `"TodoTimeManager"` `HttpClient` and provides `Url(action)` helpers.

| Service | Talks to |
|---------|---------|
| `AuthService` | `Auth` controller |
| `UserService` | `Users` controller |
| `ToDosService` | `ToDos` controller |
| `TimeLogsService` | `TimeLogs` controller |
| `TeamsService` | `Teams` controller |
| `ProjectsService` | `Projects` controller |
| `StatisticService` | `Statistic` controller |

### Services (`Services/`)

- **`CustomAuthStateProvider`** — Extends `AuthenticationStateProvider`; reads/writes tokens via `ProtectedLocalStorage`; auto-refreshes expired access tokens using `TokenRefreshService`.
- **`ToastsService`** — Singleton; manages the active toast notification list. `ShowError(msg)`, `ShowSuccess(msg)`, `ShowWarning(msg)`.
- **`ModalService`** — Scoped; opens/closes modals via `IModalService`.
- **`TokenRefreshService`** — Serializes concurrent token-refresh calls so only one outbound refresh request is made at a time.
- **`TwoFaTimerService`** — Manages the countdown timer displayed on the 2FA form.

### Helpers (`Helpers/`)

- **`CircuitServicesAccesor/`** — `CircuitServicesAccesor.cs` and `ServicesAccessorCuircutHandler.cs` provide `IServiceProvider` access outside the Blazor circuit scope.
- **`Modal/`** — `ModalParameters.cs` (key-value bag for modal params), `ModalReference.cs` (handle to await modal result).

---

## WebUI Models (`ToDoTimeManager.WebUI.Models`)

### UI-Only Enums (`Enums/`)

| Enum | Values |
|------|--------|
| `AuthPageCurrentState` | Login, Registration, TwoFA |
| `ToastType` | Success, Error, Warning |
| `InputStyle` | Default, Ghost |
| `InputIconPosition` | Left, Right |
| `ButtonStyle` | Primary, Secondary, Ghost |
| `ButtonIconPosition` | Left, Right |

### Models (`Models/`)

- `ToastModel` — Manages toast lifecycle (animation, auto-dismiss timer, disposal).
- `PendingTwoFaSessionState` — Holds the pending 2FA session on the client (userId, masked email).

---

## Configuration

### WebApi (`ToDoTimeManager.WebApi/appsettings.json`)

```json
{
  "JwtSettings": {
    "Issuer": "ToDoTimeManager",
    "Audience": "ToDoTimeManager.UI",
    "AccessTokenLifetime": "30",
    "RefreshTokenLifetime": "30"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.;Initial Catalog=TaskForge_DB;Integrated Security=True;Trust Server Certificate=True"
  },
  "TwoFactorSettings": {
    "CodeLifetimeMinutes": "5"
  },
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "SenderName": "Task Forge"
  }
}
```

- `AccessTokenLifetime` is in **minutes**; `RefreshTokenLifetime` is in **days**.
- ClockSkew is `TimeSpan.Zero` — no grace period on token expiry.
- The JWT signing key and email credentials are supplied via user secrets or environment variables — not committed to source control.

### WebUI (`ToDoTimeManager.WebUI/appsettings.json`)

```json
{
  "BaseApiUrlAddress": "https://localhost:7130/"
}
```

### CSS Build (`ToDoTimeManager.WebUI/gulpfile.js`)

- Sources: `wwwroot/css/**/*.css` (excluding `site.min.css`)
- Pipeline: concat → CleanCSS minify → rename to `site.min.css`
- Per-component CSS: `wwwroot/css/components/` (e.g. `inputs.css`, `buttons.css`)
- Per-page CSS: `wwwroot/css/pages/`
- `gulp build` — one-shot build; `gulp` (default) — build + watch.

---

## Key Conventions for Modifications

1. **New API endpoint** — add a stored procedure in `DataBase/StoredProcedures/<Entity>/` and register it in `ToDoTimeManager.DataBase.sqlproj`; implement the method on the data controller (interface in `DataAccess/DataControllers/Interfaces/`, implementation in `DataAccess/DataControllers/Implementation/`); call it from a business service in `Business/Services/Implementations/`; expose it via a controller in `WebApi/Controllers/`. Never write inline SQL.

2. **New shared type** — add to `Shared/Models/`, `Shared/DTOs/<Entity>/`, or `Shared/Enums/`. Never duplicate in any other project.

3. **New UI page** — create `WebUI/Pages/MyPage.razor` + `WebUI/Pages/MyPage.razor.cs`; use `@layout MainLayout` for authenticated pages, `@layout AuthLayout` for public ones; inherit `BaseComponent` in the code-behind.

4. **New reusable component** — place in `WebUI.Components/` under the appropriate subfolder. Inherit `BaseComponent` to get `Localizer`, `IsLoading`, and `Loading()`. Add component-specific CSS to `wwwroot/css/components/` — it is picked up by the gulp pipeline automatically.

5. **Localized strings** — add entries to `WebUI.Components/Resources/Resource.resx` (default/uk-UA) and `Resource.en-US.resx`. `BaseComponent` already injects `IStringLocalizer<Resource>` as `Localizer`.

6. **Error responses** — throw a typed exception (`ConflictException`, `ForbiddenException`, `NotFoundException`, `ValidationException`) from `Entities/Exceptions/`. `GlobalExceptionHandler` catches it on the API side. On the UI side, `ToastMessageHandler` converts HTTP errors into toast notifications automatically.

7. **Authorization checks** — use `[Authorize(Roles = "Manager,Admin")]` on controller actions for role guards; perform ownership checks inside business services using `GetCurrentUserId()` / `GetCurrentUserRole()` values passed down from the controller.

8. **Activity log tracking** — whenever a mutating operation succeeds on a `ToDo`, `TimeLog`, or `User`, call `IActivityLogsService` to write an `ActivityLog` entry.

9. **DTOs vs Models** — controllers accept/return DTOs; business services operate on shared models; entities are internal to `DataAccess`/`Business` and must never reach `WebUI`.

10. **UI-only enums** — place in `WebUI.Models/Enums/` — not in the Shared project.

11. **New utility service** — add the interface to `Business.Utils/Interfaces/` and the implementation to `Business.Utils/Implementations/`; register in `WebApi/Program.cs`.

12. **New SQL file** — every new `.sql` file (stored procedure, table, or migration) must be registered in `ToDoTimeManager.DataBase.sqlproj`.
