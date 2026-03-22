# Event Board App

Event Board is a two-project app:

- a .NET backend in [`backend/`](/backend) that owns API contracts, business rules, and persistence
- a Next.js web app in [`webapp/`](/webapp) that owns the user-facing experience on top of that API

When backend and webapp behavior disagree, the backend contract is the source of truth unless the contract is intentionally being changed.

## What This Project Includes

- ASP.NET Core Web API
- Clean/layered backend structure
- Next.js App Router web app
- SQLite persistence with EF Core
- Automatic migrations and seed data on startup
- Swagger UI outside production
- xUnit test suite with SQLite-backed persistence tests
- ESLint-based frontend verification

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
├── webapp/
│   ├── app/
│   ├── components/
│   ├── features/
│   ├── lib/
│   └── public/
├── docs/
│   ├── technical-decisions.md
│   └── footguns-and-guardrails.md
├── AGENTS.md
└── CLAUDE.md
```

## Architecture

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

The web app follows a feature-oriented Next.js structure:

- `webapp/app`
  - routes, layouts, loading/error boundaries, page-level server composition
- `webapp/features`
  - feature queries, Server Actions, feature types, feature UI
- `webapp/components`
  - shared presentational UI primitives
- `webapp/lib`
  - shared API client code and cross-feature utilities

### Architecture Decisions That Matter

- Controllers stay thin and delegate behavior to services.
- Business rules live in the application layer.
- EF Core queries, transactions, and write mechanics stay in repositories.
- The API preserves snake_case JSON and a consistent `ApiErrorDto` error envelope.
- Time-based logic uses `TimeProvider`.
- Registration concurrency is resolved at the database write boundary, not by read-then-write checks in controllers or services.
- The webapp consumes backend contracts and should not silently redefine API semantics in UI-only mapping code.
- Shared frontend components stay presentation-focused; feature-specific behavior belongs in `webapp/features`.

More detail lives in:

- [`docs/technical-decisions.md`](/docs/technical-decisions.md)
- [`docs/footguns-and-guardrails.md`](/docs/footguns-and-guardrails.md)

## Requirements

- .NET SDK 10.0
- Node.js 20+
- npm
- `make` if you want to use the provided backend shortcuts

## How To Run

### Backend

```bash
cd backend
make run
```

Or without `make`:

```bash
cd backend
dotnet run --project src/EventBoard.Api
```

### Webapp

The webapp reads the backend origin from a server-side `API_BASE_URL` environment variable.

From the repository root:

```bash
cd webapp
API_BASE_URL=http://localhost:5153 npm run dev
```

If your backend starts on a different port, use that origin instead.

### Running Both Together

1. Start the backend from [`backend/`](/backend).
2. Start the webapp from [`webapp/`](/webapp) with `API_BASE_URL` pointing at the backend.
3. Open [http://localhost:3000](http://localhost:3000).

### What Happens On Startup

- EF Core migrations are applied automatically.
- Seed data is inserted automatically.
- Swagger is enabled in non-production environments.
- HTTPS redirection is skipped in development.

### Local Development Defaults

- API project: [`backend/src/EventBoard.Api`](/backend/src/EventBoard.Api)
- Default development database: `backend/src/EventBoard.Api/eventboard_dev.db` (SQLite file created locally; not committed)
- Default connection string is configured in:
  - [`backend/src/EventBoard.Api/appsettings.json`](/backend/src/EventBoard.Api/appsettings.json)
  - [`backend/src/EventBoard.Api/appsettings.Development.json`](/backend/src/EventBoard.Api/appsettings.Development.json)
- Webapp default dev URL: [http://localhost:3000](http://localhost:3000)
- Webapp API configuration: `API_BASE_URL`

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

Run frontend verification with:

```bash
cd webapp
npm run lint
```

If a change touches both projects, run both the backend tests and frontend lint step.

## API Overview

Current HTTP surface:

- `GET /api/events`
- `GET /api/events/{id}`
- `POST /api/events/{eventId}/registrations`

## Project Guidance

Before making non-trivial changes, read:

- [`AGENTS.md`](/AGENTS.md)
- [`CLAUDE.md`](/CLAUDE.md)
- [`docs/technical-decisions.md`](/docs/technical-decisions.md)
- [`docs/footguns-and-guardrails.md`](/docs/footguns-and-guardrails.md)
