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

Five projects in the solution:

- **ToDoTimeManager.WebApi** — ASP.NET Core 8 REST API (auth, todos, time logs, teams, projects, statistics, user management)
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
2. Tokens are stored in `ProtectedLocalStorage` on the browser via `TokenProtectedStorageHelper`.
3. `TokenMessageHandler` (WebUI) injects the Bearer token into every outgoing HTTP request.
4. `ToastMessageHandler` (WebUI) intercepts error responses and surfaces them as toast notifications.
5. `CustomAuthStateProvider` manages Blazor auth state and triggers automatic token refresh when the access token is expired but the refresh token is still valid.
6. JWT settings (key, issuer, lifetimes) live in `WebApi/appsettings.json`. ClockSkew is `TimeSpan.Zero` — tokens expire exactly at their `ValidTo` time.

### Authorization Model

- Two roles: `User` (0) and `Admin` (1), defined in `Shared/Enums/UserRole.cs`.
- Admins have unrestricted access to all resources.
- Regular users can only read/write their own resources (own to-dos, time logs, profile).
- Resource ownership is verified inside business services, not controllers.
- `TeamMemberRole` (Member=0, Owner=1) governs per-team permissions for team/project operations.

### Key Structural Patterns

- **All database access** goes through `DbAccessService` (a generic Dapper wrapper). Services and data controllers never write raw SQL — they call stored procedures only.
- **All WebUI HTTP calls** use typed service classes inheriting from `BaseHttpService` (`WebUI/Services/HttpServices/`). `BaseHttpService` creates the named `"TodoTimeManager"` `HttpClient`.
- **Shared contracts** (models, DTOs, enums) live exclusively in the Shared project — never duplicated in WebApi or WebUI.
- **Entities** (`WebApi/Entities/`) are Dapper-mapped types separate from the shared models. Each entity exposes a `To[Model]()` conversion method.
- **Localization** is built in; resource strings are in `WebUI/Localization/Resource.resx` with `uk-UA` (default) and `en-US` variants.
- **Error handling**: `GlobalExceptionHandler` middleware on the API; `ToastMessageHandler` + `ToastsService` on the UI.
- **CircuitServicesAccesor**: A Blazor Server workaround that provides access to scoped services (e.g., `ProtectedLocalStorage`) from within `CustomAuthStateProvider`.

---

## Notable Directories

| Path | Purpose |
|------|---------|
| `WebApi/Controllers/` | REST endpoints |
| `WebApi/Services/Implementations/` | Business logic |
| `WebApi/Services/DataControllers/` | DB access layer (interfaces + implementations) |
| `WebApi/Services/DataControllers/DbAccessServices/` | Generic Dapper `DbAccessService` |
| `WebApi/Entities/` | Database-mapped entity types (separate from Shared models) |
| `WebApi/AdditionalComponents/` | `CustomException`, `AuthResponsesOperationFilter` |
| `WebApi/Middleware/` | `GlobalExceptionHandler` |
| `WebApi/Utils/` | `PasswordHelperService`, `JwtGeneratorService` |
| `WebUI/Pages/` | Routed Blazor pages |
| `WebUI/Components/Base/` | `BaseComponent`, `BaseAuthForm` — abstract base classes |
| `WebUI/Components/Pages/AuthPage/` | Auth sub-components: `LoginForm`, `RegisterForm`, `TwoFaForm` |
| `WebUI/Components/Modals/` | Modal dialog components |
| `WebUI/Components/Shared/` | Reusable UI components (Input, Button, CheckInput, CustomDropdown, StepsComponent, icons, etc.) |
| `WebUI/Components/Toast/` | Toast notification components |
| `WebUI/Handlers/` | HTTP message handlers |
| `WebUI/Models/Enums/` | WebUI-only enums (InputStyle, ButtonStyle, AuthPageCurrentState, etc.) |
| `WebUI/Services/HttpServices/` | Typed API client services |
| `WebUI/Services/Implementations/` | `CustomAuthStateProvider`, `ToastsService` |
| `WebUI/Services/CircuitServicesAccesor/` | Scoped-service accessor for Blazor circuits |
| `WebUI/Utils/` | `TokenProtectedStorageHelper`, `PageTitleHelper`, `CultureInfoHelper` |
| `WebUI/Localization/` | `Resource.resx`, `Resource.en-US.resx`, `Resource.uk-UA.resx` |
| `WebUI/Controllers/` | `CultureController` (MVC, handles culture switching) |
| `WebUI/wwwroot/css/components/` | Per-component CSS files (inputs.css, etc.) |
| `Shared/Models/` | Domain models (User, ToDo, TimeLog, Team, Project, …) |
| `Shared/DTOs/` | Request/response DTOs |
| `Shared/Enums/` | `ToDoStatus`, `UserRole`, `TimeFilter`, `TeamMemberRole` |
| `Shared/Utils/` | `JwtTokenHelper`, `NotEmptyGuidAttribute` |
| `DataBase/Tables/` | SQL table creation scripts |
| `DataBase/StoredProcedures/` | All stored procedures, grouped by entity |

---

## API Endpoints

All controllers live under `api/[Controller]` (e.g. `api/Auth`, `api/ToDos`).

### AuthController — `[AllowAnonymous]`
| Verb | Route | Description |
|------|-------|-------------|
| POST | `Auth/Login` | Authenticate with credentials; returns `TokenModel` |
| POST | `Auth/RefreshToken` | Exchange refresh token for new access token |

### UsersController
| Verb | Route | Authorization | Description |
|------|-------|---------------|-------------|
| GET | `Users/GetAll` | Admin | Get all users |
| GET | `Users/GetById/{id}` | Auth | Get user by ID (admin or self) |
| GET | `Users/GetByUsername/{userName}` | Admin | Get user by username |
| GET | `Users/GetByEmail/{email}` | Admin | Get user by email |
| GET | `Users/GetByLoginParameter/{loginParameter}` | Admin | Get user by username or email |
| POST | `Users/Create` | AllowAnonymous | Register new user |
| PUT | `Users/Update` | Auth | Update own profile |
| PUT | `Users/ChangeRole/{id}` | Admin | Change user's role |
| DELETE | `Users/Delete/{id}` | Admin | Delete user |

### ToDosController
| Verb | Route | Authorization | Description |
|------|-------|---------------|-------------|
| GET | `ToDos/GetAll` | Admin | Get all to-dos |
| GET | `ToDos/GetById/{id}` | Auth | Get to-do (admin or assignee) |
| GET | `ToDos/GetByUserId/{userId}` | Auth | Get user's to-dos (admin or self) |
| POST | `ToDos/Create` | Auth | Create new to-do |
| PUT | `ToDos/Update` | Auth | Update to-do (admin or assignee) |
| DELETE | `ToDos/Delete/{id}` | Auth | Delete to-do (admin or assignee) |

### TimeLogsController
| Verb | Route | Authorization | Description |
|------|-------|---------------|-------------|
| GET | `TimeLogs/GetAll` | Admin | Get all time logs |
| GET | `TimeLogs/GetById/{id}` | Auth | Get log (admin or owner) |
| GET | `TimeLogs/GetByToDoId/{toDoId}` | Auth | Get logs for a to-do |
| GET | `TimeLogs/GetByUserId/{userId}` | Auth | Get user's logs (admin or self) |
| GET | `TimeLogs/GetByUserIdAndToDoId/{userId}/{toDoId}` | Auth | Get logs for user + to-do |
| POST | `TimeLogs/Create` | Auth | Create time log entry |
| PUT | `TimeLogs/Update` | Auth | Update log (admin or owner) |
| DELETE | `TimeLogs/Delete/{id}` | Auth | Delete log (admin or owner) |

### TeamsController
| Verb | Route | Authorization | Description |
|------|-------|---------------|-------------|
| GET | `Teams/GetAll` | Admin | Get all teams |
| GET | `Teams/GetById/{id}` | Auth | Get team (admin or member) |
| GET | `Teams/GetToDosByTeamId/{teamId}` | Auth | Get team's to-dos (admin or member) |
| GET | `Teams/GetMyTeams` | Auth | Get current user's teams |
| PUT | `Teams/Update` | Auth | Update team (admin or creator/owner) |
| POST | `Teams/AddMember` | Auth | Add member to team |
| DELETE | `Teams/RemoveMember/{teamId}/{userId}` | Auth | Remove member from team |
| DELETE | `Teams/Delete/{id}` | Admin | Delete team |

### ProjectsController
| Verb | Route | Authorization | Description |
|------|-------|---------------|-------------|
| GET | `Projects/GetAll` | Admin | Get all projects |
| GET | `Projects/GetById/{id}` | Auth | Get project (admin or member via team) |
| GET | `Projects/GetToDosByProjectId/{projectId}` | Auth | Get project's to-dos |
| GET | `Projects/GetMyProjects` | Auth | Get current user's projects |
| PUT | `Projects/Update` | Auth | Update project (admin or creator) |
| POST | `Projects/AddTeam` | Auth | Associate team with project |
| DELETE | `Projects/RemoveTeam/{projectId}/{teamId}` | Auth | Remove team from project |
| DELETE | `Projects/Delete/{id}` | Admin | Delete project |

### StatisticController
| Verb | Route | Authorization | Description |
|------|-------|---------------|-------------|
| GET | `Statistic/GetToDoCountStatisticsOfAllTimeByUserId/{userId}` | Auth | To-do counts by status (admin or self) |
| POST | `Statistic/GetMainPageStatistic` | Auth | Dashboard stats (time logs, upcoming due dates, status counts) |

---

## Shared Project

### Models (`Shared/Models/`)
| Model | Key Fields |
|-------|-----------|
| `User` | Id, UserName, Email, Password, UserRole |
| `ToDo` | Id, NumberedId, Title, Description, CreatedAt, DueDate, Status, AssignedTo, TeamId, ProjectId, DisplayDueDate (computed) |
| `TimeLog` | Id, ToDoId, UserId, HoursSpent (TimeSpan), LogDate, LogDescription |
| `Team` | Id, Name, Description, CreatedAt, CreatedBy, MemberCount |
| `Project` | Id, Name, Description, CreatedAt, CreatedBy, TeamCount |
| `TeamMember` | Id, TeamId, UserId, Role |
| `ProjectTeam` | Id, ProjectId, TeamId |
| `TokenModel` | AccessToken, RefreshToken, RefreshTokenExpiresAt |
| `LoginUser` | LoginParameter (username or email), Password |
| `MainPageStatisticModel` | TimeLogsForGivenTime, TimeLogsForThisMonth, DueDateTasks (Dictionary\<DateTime, ToDo\>), ToDoStatuses |
| `MainPageStatisticRequest` | UserId, TimeFilter |
| `ToDoCountStatisticsOfAllTime` | Statistics by status |

### DTOs (`Shared/DTOs/`)
- `CreateUserRequestDto`, `UpdateUserRequestDto`, `UserResponseDto` (no password), `ChangeUserRoleRequestDto`
- `CreateProjectRequestDto`, `UpdateProjectRequestDto`, `ProjectResponseDto`
- `CreateTeamRequestDto`, `UpdateTeamRequestDto`, `TeamResponseDto`
- `ProjectTeamUpsertRequestDto`, `TeamMemberUpsertRequestDto`
- `ToDoUpsertRequestDto`
- `TimeLogUpsertRequestDto`

### Enums (`Shared/Enums/`)
- `UserRole` — User (0), Admin (1)
- `ToDoStatus` — New, InProgress, Completed, OnHold, Cancelled
- `TeamMemberRole` — Member (0), Owner (1)
- `TimeFilter` — AllTime, DayAgo, WeekAgo, MonthAgo, YearAgo

### Utils (`Shared/Utils/`)
- `JwtTokenHelper` — Parses UserId and UserRole claims from an access token string.
- `NotEmptyGuidAttribute` — `ValidationAttribute` that rejects `Guid.Empty`.

---

## Database

### Tables (`DataBase/Tables/`)

All primary keys are `UNIQUEIDENTIFIER`. All FKs reference other tables by GUID.

| Table | Columns |
|-------|---------|
| `Users` | Id, Username, Email, Password, UserRole (INT) |
| `ToDos` | Id, NumberedId (INT), Title, Description, CreatedAt, DueDate, Status (INT), AssignedTo→Users, TeamId→Teams, ProjectId→Projects |
| `Teams` | Id, Name, Description, CreatedAt, CreatedBy→Users |
| `TeamMembers` | Id, TeamId→Teams, UserId→Users, Role (INT), UQ(TeamId, UserId) |
| `Projects` | Id, Name, Description, CreatedAt, CreatedBy→Users |
| `ProjectTeams` | Id, ProjectId→Projects, TeamId→Teams, UQ(ProjectId, TeamId) |
| `TimeLogs` | Id, ToDoId→ToDos, UserId→Users, HoursSpent (TIME), LogDate, LogDescription |

### Stored Procedures (`DataBase/StoredProcedures/`)

Naming convention: `sp_[EntityName]_[Operation]`

| Entity | Procedures |
|--------|-----------|
| Users | GetAll, GetById, GetbyUsername, GetByEmail, GetByLoginParameter, Create, Update, DeleteById |
| ToDos | GetAll, GetById, GetByAssignedTo, GetByTeamId, GetByProjectId, GetByNearestDueDateByUserId, GetCountByUserIdAndStatus, Create, Update, DeleteByid |
| TimeLogs | GetAll, GetById, GetByToDoId, GetByUserId, GetByUserIdAndToDoId, GetByUserIdAndTime, Create, Update, DeleteById |
| Teams | GetAll, GetById, GetByUserId, Create, Update, DeleteById |
| TeamMembers | GetById, GetByTeamId, GetByTeamIdAndUserId, Create, DeleteByTeamIdAndUserId |
| Projects | GetAll, GetById, GetByUserId, Create, Update, DeleteById |
| ProjectTeams | GetByProjectId, GetByProjectIdAndTeamId, Create, DeleteByProjectIdAndTeamId |

---

## WebUI Pages

| File | Route | Description |
|------|-------|-------------|
| `AuthPage.razor` | `/auth` | Multi-step auth: Login → (2FA), Registration → (2FA) with animated slide transitions |
| `MainPage.razor` | `/dashboard` | Dashboard: time stats, heat-map calendar (color-coded by hours), upcoming due-date tasks |
| `TasksPage.razor` | `/tasks` | To-do list with status filters and CRUD |
| `TaskDetailsPage.razor` | `/taskDetails/{taskId}` | Single to-do detail view with time log history |
| `TimeLogsPage.razor` | `/timelogs` | Time entry management |
| `ProfilePage.razor` | `/profile` | User account settings |

Pages use code-behind files (`.razor.cs`). Layouts: `MainLayout.razor` (authenticated), `AuthLayout.razor` (unauthenticated).

### AuthPage Flow

`AuthPage` manages state via `AuthPageCurrentState` enum (`Login`, `Registration`, `TwoFA`) and drives animated CSS slide transitions between three sub-components:

```
AuthPage.razor
  └── BaseAuthForm (template: icon, title, subtitle, step indicator)
        ├── LoginForm      — credentials entry (step 1 of login)
        ├── RegisterForm   — account creation (step 1 of registration)
        └── TwoFaForm      — 2FA code entry (step 2 of both flows)
```

Animation uses CSS classes (`--active`, `--hidden-right`, `--hidden-left`, `--exiting-*`, `--entering-*`) applied with a 450 ms transition.

---

## WebUI Components

### Base (`Components/Base/`)
- **BaseComponent.cs** — Abstract base for all components. Provides injected `Localizer`, `IsLoading` state, `Loading(Func<Task>)` async wrapper, and `GetPageTitle(string)` that returns a `RenderFragment` with a localized title + "TaskForge" suffix.
- **BaseAuthForm.razor** — Template layout for multi-step auth forms. Parameters: `FormContent` (RenderFragment), `Steps` (List\<string\>), `CurrentStep`, `MainIcon`, `MainText`, `SubText`.

### Shared (`Components/Shared/`)
- **Input.razor** — Validated text input with icon support. Key parameters: `Value`/`ValueChanged`, `Type`, `IsPassword` (toggleable eye button), `Icon`, `IconPosition` (Left/Right), `UseValidation`, `ValidationFunc`. `StringValidationHelper` provides pre-built validators: `DefaultValidation`, `EmailValidation`, `PasswordValidation`, `UsernameValidation`, `ConfirmPasswordValidation`, `EmailOrUsernameValidation`.
- **Button.razor** — Reusable button with icon, style variants (`Primary`, `Secondary`, `Ghost`), and optional `IsLoading` spinner.
- **CheckInput.razor** — Checkbox with label.
- **CustomDropdown.razor** — Searchable dropdown with single/multi-select, icons, and templated items. Uses `Input` internally for the search field.
- **StepsComponent.razor** — Multi-step form progress indicator. Renders current step with i18n label + "In Progress" status; completed steps show a checkmark.
- `CultureSelector.razor` — Language switcher (en-US / uk-UA), posts to `CultureController`.
- `Loader.razor` — Spinner displayed during async operations.
- `Divider.razor` — Visual separator.
- `Icons/` — Individual SVG icon components (Bootstrap Icons as Razor components).

### Modals (`Components/Modals/`)
- **TaskCreateEditModal.razor** — Create/edit to-do (Title, Description, Status, DueDate). Parameters: `Show`, `Title`, `Task`, `IsEditMode`. Emits `ModalResult`.
- **LogTimeModal.razor** — Create/edit time log (TaskNumber, HoursSpent, LogDescription). Parameters: `Show`, `Title`, `Task`, `BlockTaskNumberInput`, `IsEditMode`, `ExistingTimeLog`. Emits `ModalResult` with `TimeLog` and `TaskNumber`.
- **ConfirmationModal.razor** — Generic confirmation dialog.
- **UserEditModal.razor** — User profile editing.

### Toast System (`Components/Toast/`)
- `Toast.razor` — Single toast notification.
- `ToastBox.razor` / `ToastBox.razor.cs` — Container that renders active toasts.
- `ToastsService` (singleton) — `ShowError(msg)` / `ShowSuccess(msg)` used throughout pages.

---

## WebUI Services

### HTTP Services (`Services/HttpServices/`)

`BaseHttpService` creates the named `"TodoTimeManager"` `HttpClient` and provides `Url(action)` helpers.

| Service | WebApi controller |
|---------|------------------|
| `AuthService` | `Auth` |
| `UserService` | `Users` |
| `ToDosService` | `ToDos` |
| `TimeLogsService` | `TimeLogs` |
| `StatisticService` | `Statistic` |

### Other Services (`Services/Implementations/`)
- `CustomAuthStateProvider` — Extends `AuthenticationStateProvider`; reads/writes tokens via `ProtectedLocalStorage`; auto-refreshes expired access tokens.
- `ToastsService` — Singleton; manages the active toast notification list.

### Circuit Services (`Services/CircuitServicesAccesor/`)
- `CircuitServicesAccesor` — Provides `IServiceProvider` access from outside the Blazor circuit scope (used in `CustomAuthStateProvider`).
- `ServicesAccessorCircuitHandler` — `CircuitHandler` that sets/clears the accessor on connect/disconnect.

---

## WebUI-Only Enums (`WebUI/Models/Enums/`)

These are UI-layer enums that do **not** belong in the Shared project:

| Enum | Values |
|------|--------|
| `InputStyle` | Default, Ghost |
| `InputIconPosition` | Left, Right |
| `ButtonStyle` | Primary, Secondary, Ghost |
| `ButtonIconPosition` | Left, Right |
| `AuthPageCurrentState` | Login, Registration, TwoFA |

---

## Configuration

### WebApi (`WebApi/appsettings.json`)
```json
{
  "JwtSettings": {
    "Key": "NbpX8eP2W5pLdSwxkJf3eI2/TsR9aNHJ",
    "Issuer": "ToDoTimeManager",
    "Audience": "ToDoTimeManager.UI",
    "AccessTokenLifetime": "15",
    "RefreshTokenLifetime": "14"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.;Initial Catalog=ToDoTimeManager;Integrated Security=True;Trust Server Certificate=True"
  }
}
```
- AccessTokenLifetime is in **minutes**; RefreshTokenLifetime is in **days**.
- ClockSkew is `TimeSpan.Zero` — no grace period on token expiry.

### WebUI (`WebUI/appsettings.json`)
```json
{
  "BaseApiUrlAddress": "https://localhost:7130/"
}
```

### CSS Build (`WebUI/gulpfile.js`)
- Sources: `wwwroot/css/**/*.css` (excluding `site.min.css`)
- Pipeline: concat → CleanCSS minify → rename to `site.min.css`
- Per-component CSS files live in `wwwroot/css/components/` (e.g. `inputs.css`)
- `gulp build` — one-shot build; `gulp` (default) — build + watch.

---

## Key Conventions for Modifications

1. **New API endpoint** → add a stored procedure in `DataBase/StoredProcedures/`, implement a data controller method (interface + implementation), call it from a business service, expose via controller. Never write inline SQL.
2. **New shared type** → add to `Shared/Models/` or `Shared/DTOs/`; never duplicate in WebApi or WebUI.
3. **New UI page** → create `Pages/MyPage.razor` + `Pages/MyPage.razor.cs`; use `MainLayout` for authenticated pages, `AuthLayout` for public ones.
4. **New reusable component** → extend `BaseComponent` to inherit `Localizer`, `IsLoading`, and `Loading()`. Add component-specific CSS under `wwwroot/css/components/` — it will be picked up by the gulp pipeline automatically.
5. **Localized strings** → add entries to `WebUI/Localization/Resource.resx` (default/uk-UA) and `Resource.en-US.resx`; inject `IStringLocalizer<Resource>` in the component.
6. **Error responses** → throw a `CustomException` in service code; `GlobalExceptionHandler` middleware catches it on the API side. On the UI side, `ToastMessageHandler` converts HTTP errors into toast notifications automatically.
7. **Authorization checks** → role guard with `[Authorize(Roles="Admin")]` on the controller action; ownership checks (is this the user's own resource?) inside the business service.
8. **DTOs vs Models** → controllers accept/return DTOs; services operate on shared models; entities are internal to `WebApi` and never leak to `WebUI`.
9. **UI-only enums** → place in `WebUI/Models/Enums/` — not in the Shared project.
