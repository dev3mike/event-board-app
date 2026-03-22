# CLAUDE.md

## Purpose

Treat the .NET backend under `backend/` as the architectural source of truth unless a task explicitly says otherwise.

## Read This First

- [docs/technical-decisions.md](/docs/technical-decisions.md)
- [docs/footguns-and-guardrails.md](/docs/footguns-and-guardrails.md)
- [AGENTS.md](/AGENTS.md)

## Backend Shape

The backend follows a layered structure (Clean Architecture):

- `backend/src/EventBoard.Api`
  - HTTP transport, middleware, startup, Swagger, request DTOs
- `backend/src/EventBoard.Application`
  - use cases, services, repository interfaces, DTO shaping
- `backend/src/EventBoard.Domain`
  - entities, domain exceptions, shared helpers
- `backend/src/EventBoard.Infrastructure`
  - EF Core, repositories, migrations, seeding

Preserve dependency direction:

- `Api -> Application -> Domain`
- `Infrastructure` implements persistence for `Application` and depends on `Application` + `Domain`

## Core Conventions

- Keep controllers thin.
- Keep business rules in application services.
- Keep EF Core query/update mechanics in repositories.
- Preserve snake_case JSON and the current `ApiErrorDto` error envelope.
- Use `TimeProvider` for time-based behavior.
- Pass `CancellationToken` through async flows.
- Keep business logic testable without HTTP setup when possible.

## Verification

After backend changes, run:

```bash
dotnet test backend/EventBoard.sln
```

## Agent Expectations

- Inspect the owning layer before editing.
- Make the smallest change that matches the architecture.
- If a task requires breaking a documented boundary, call it out explicitly.
- Do not treat every existing pattern as ideal; use the deeper docs for decisions, risks, and exceptions.

## Required Close-Out Note

Before finishing a task, include a short close-out note that states:

- which instruction docs you followed
- one repo-specific constraint or guardrail you accounted for
- whether the task introduced a new technical decision or footgun worth documenting, and if not, say no
