# Event Board App

Event Board is a .NET backend for browsing events and creating registrations. The current repository is backend-first: the API, application logic, persistence, and tests all live under [`backend/`](/backend).

## What This Project Includes

- ASP.NET Core Web API
- Clean/layered backend structure
- SQLite persistence with EF Core
- Automatic migrations and seed data on startup
- Swagger UI outside production
- xUnit test suite with SQLite-backed persistence tests

## Repository Layout

```text
.
├── backend/
│   ├── Makefile
│   ├── EventBoard.sln
│   ├── src/
│   │   ├── EventBoard.Api
│   │   ├── EventBoard.Application
│   │   ├── EventBoard.Domain
│   │   └── EventBoard.Infrastructure
│   └── tests/
│       └── EventBoard.Tests
├── docs/
│   ├── technical-decisions.md
│   └── footguns-and-guardrails.md
├── AGENTS.md
└── CLAUDE.md
```

## Backend Architecture

The backend follows a layered structure with explicit ownership boundaries:

- `EventBoard.Api`
  - HTTP transport, controllers, middleware, Swagger, request DTOs
- `EventBoard.Application`
  - business rules, orchestration, DTO shaping, service interfaces
- `EventBoard.Domain`
  - entities, domain exceptions, shared domain helpers
- `EventBoard.Infrastructure`
  - EF Core, `DbContext`, repository implementations, migrations, seeding

Dependency direction is intentionally kept narrow:

```text
Api -> Application -> Domain
Infrastructure -> Application + Domain
```

### Architecture Decisions That Matter

- Controllers stay thin and delegate behavior to services.
- Business rules live in the application layer.
- EF Core queries, transactions, and write mechanics stay in repositories.
- The API preserves snake_case JSON and a consistent `ApiErrorDto` error envelope.
- Time-based logic uses `TimeProvider`.
- Registration concurrency is resolved at the database write boundary, not by read-then-write checks in controllers or services.

More detail lives in:

- [`docs/technical-decisions.md`](/docs/technical-decisions.md)
- [`docs/footguns-and-guardrails.md`](/docs/footguns-and-guardrails.md)

## Requirements

- .NET SDK 10.0
- `make` if you want to use the provided backend shortcuts

## How To Run

From the repository root:

```bash
cd backend
make run
```

Or without `make`:

```bash
cd backend
dotnet run --project src/EventBoard.Api
```

### What Happens On Startup

- EF Core migrations are applied automatically.
- Seed data is inserted automatically.
- Swagger is enabled in non-production environments.
- HTTPS redirection is skipped in development.

### Local Development Defaults

- API project: [`backend/src/EventBoard.Api`](/backend/src/EventBoard.Api)
- Default development database: [`backend/src/EventBoard.Api/eventboard_dev.db`](/backend/src/EventBoard.Api/eventboard_dev.db)
- Default connection string is configured in:
  - [`backend/src/EventBoard.Api/appsettings.json`](/backend/src/EventBoard.Api/appsettings.json)
  - [`backend/src/EventBoard.Api/appsettings.Development.json`](/backend/src/EventBoard.Api/appsettings.Development.json)

Once the app is running, Swagger is typically available at:

- [http://localhost:5098/swagger](http://localhost:5098/swagger)
- [https://localhost:7166/swagger](https://localhost:7166/swagger)

If those ports differ on your machine, check the ASP.NET startup logs or [`launchSettings.json`](/backend/src/EventBoard.Api/Properties/launchSettings.json).

## Makefile Shortcuts

The project currently includes a backend-local Makefile at [`backend/Makefile`](/backend/Makefile).

Available targets:

- `make help` - show available targets
- `make run` - run the backend API
- `make test` - run the backend test suite

From the repo root, you can also call them like this:

```bash
make -C backend help
make -C backend run
make -C backend test
```

## Testing

Run the backend test suite with:

```bash
dotnet test backend/EventBoard.sln
```

Or via the Makefile:

```bash
make -C backend test
```

The test suite uses SQLite-backed tests where real EF Core relational behavior matters, especially around registration writes and concurrency-sensitive persistence behavior.

## API Overview

Current HTTP surface:

- `GET /api/events`
- `GET /api/events/{id}`
- `POST /api/events/{eventId}/registrations`

## Project Guidance

Before making non-trivial backend changes, read:

- [`AGENTS.md`](/AGENTS.md)
- [`CLAUDE.md`](/CLAUDE.md)
- [`docs/technical-decisions.md`](/docs/technical-decisions.md)
- [`docs/footguns-and-guardrails.md`](/docs/footguns-and-guardrails.md)
