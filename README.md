# Café Employee Manager

Full-stack web application for managing cafés and employees.

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | .NET 8, ASP.NET Core, MediatR, Autofac, FluentValidation, EF Core 8 |
| Database | PostgreSQL 16 |
| Frontend | React 18, Vite, TypeScript, Ant Design 5, AG Grid, TanStack Query, Tailwind CSS, Dayjs |
| Infra | Docker, Docker Compose |

## Architecture

### Backend — Clean Architecture + CQRS

```
CafeManager.Domain         ← Entities, repository interfaces. Zero dependencies.
CafeManager.Application    ← CQRS commands/queries via MediatR. FluentValidation.
CafeManager.Infrastructure ← EF Core + PostgreSQL. Repository implementations.
CafeManager.API            ← ASP.NET controllers. Autofac module. Middleware.
```

**Key patterns:**
- **CQRS** — every operation is a distinct `Command` (write) or `Query` (read), each with its own handler
- **Mediator** — controllers call `_mediator.Send(...)` only; completely decoupled from business logic
- **Autofac** — `ApplicationModule` registers repositories via Autofac; MediatR handlers and validators are registered via `AddMediatR` and `AddValidatorsFromAssembly`
- **Validation pipeline** — `ValidationBehavior<T>` runs FluentValidation automatically before every handler
- **RFC 7807 ProblemDetails** — all errors returned as structured `application/problem+json`
- **Serilog** — structured JSON logging with daily file rotation
- **Health check** — `GET /health` for container orchestrators

### Frontend — Feature-based structure

```
src/
  api/          ← Axios functions per resource; centralised error interceptor
  components/   ← Reusable: ErrorBoundary, QueryStateWrapper, PageHeader, TableActionButtons, UnsavedBanner, ReusableTextInput
  hooks/        ← TanStack Query hooks (data fetching, caching, mutations)
  pages/        ← Page components (thin — delegate to hooks and components)
  types/        ← Shared TypeScript interfaces
```

---

## Running Locally

### Option 1 — Docker Compose (recommended)

```bash
# 1. Copy the env template and set a password
cp .env.example .env

# 2. Build and start all services
docker compose down -v
docker compose up --build
```

| Service | URL |
|---|---|
| Frontend | http://localhost:3000 |
| Backend API | http://localhost:5000 |
| Swagger UI | http://localhost:5000/swagger |
| Health check | http://localhost:5000/health |

### Option 2 — Manual (requires .NET 8 SDK, Node 22+, PostgreSQL 16)

**Backend:**
```bash
cd backend
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=cafe_manager;Username=postgres;Password=postgres"
dotnet run --project src/CafeManager.API
```

**Frontend:**
```bash
cd frontend
npm install
npm run dev   # http://localhost:5173
```

---

## API Reference

All endpoints are versioned under `/api/v1/`.

| Method | Path | Description |
|---|---|---|
| GET | `/api/v1/cafes?location=` | List cafés (sorted by employee count, optional partial location filter) |
| POST | `/api/v1/cafes` | Create a café (multipart/form-data) |
| PUT | `/api/v1/cafes/{id}` | Update a café (multipart/form-data) |
| DELETE | `/api/v1/cafes/{id}` | Delete a café and all its employees |
| GET | `/api/v1/employees?cafe=` | List employees (sorted by days worked, optional café filter) |
| POST | `/api/v1/employees` | Create an employee |
| PUT | `/api/v1/employees/{id}` | Update an employee |
| DELETE | `/api/v1/employees/{id}` | Delete an employee |
| GET | `/health` | Health check (database ping) |

Errors follow RFC 7807 `application/problem+json`.

---

## Validation Rules

| Field | Rule |
|---|---|
| Café name | Required, 6–10 characters |
| Café description | Required, max 256 characters |
| Café logo | Optional, max 2 MB, must be a valid image (JPEG/PNG/GIF/WebP) |
| Employee name | Required, 6–30 characters |
| Employee email | Required, valid email format, unique |
| Employee phone | Required, starts with 8 or 9, exactly 8 digits (Singapore format) |
| Employee gender | Required, Male or Female |

---

## Database Schema

```
Cafes           id (UUID PK), name (≤10), description (≤256), logo?, location
Employees       id (UIXXXXXXX PK), name (≤30), email_address (unique), phone_number, gender
CafeEmployees   cafe_id (FK→Cafes, cascade delete), employee_id (FK→Employees, unique), start_date
```

The unique constraint on `employee_id` in `CafeEmployees` enforces the one-café-per-employee rule at the database level. Deleting a café also deletes all employees assigned to it.

---

## Notes

- **Secrets** — never committed. Set `POSTGRES_PASSWORD` in `.env` before running (see `.env.example`).
- **Database schema** — created automatically on first startup via `EnsureCreatedAsync`. No migration files needed.
- **Seed data** — 3 cafés and 5 employees are inserted automatically on first run if the database is empty.
- **File uploads** — logos are stored in a named Docker volume (`uploads_data`) so they persist across container restarts.
- **CORS** — configured via the `Cors__AllowedOrigins` environment variable, not hardcoded.
- **API versioning** — all routes are under `/api/v1/`. Future versions can be added without breaking existing clients.
