# CLAUDE.md

## Purpose

Treat this repo as a two-project system:

- `backend/` owns API contracts, business rules, persistence behavior, and transport semantics
- `webapp/` owns UI composition, server-rendered UX, and frontend state transitions built on top of the backend contract

When backend and webapp disagree, treat the backend contract as the source of truth unless a task explicitly changes that contract.

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

## Webapp Shape

The webapp follows a feature-oriented Next.js structure:

- `webapp/app`
  - routes, layouts, loading/error boundaries, top-level server-rendered composition
- `webapp/features`
  - feature-specific queries, Server Actions, types, and UI components
- `webapp/components`
  - shared presentation primitives and reusable UI building blocks
- `webapp/lib`
  - shared API client code and cross-feature utilities

Preserve dependency direction:

- route segments compose feature modules
- shared UI stays generic and does not absorb feature-specific business behavior
- the webapp consumes backend contracts; it does not redefine them ad hoc in UI code

## Core Conventions

- Keep controllers thin.
- Keep business rules in application services.
- Keep EF Core query/update mechanics in repositories.
- Preserve snake_case JSON and the current `ApiErrorDto` error envelope.
- Use `TimeProvider` for time-based behavior.
- Pass `CancellationToken` through async flows.
- Keep business logic testable without HTTP setup when possible.
- Keep data fetching and mutations in Server Components, feature queries, or Server Actions rather than casual client-side fetches.
- Keep shared UI components presentational; feature behavior belongs in `webapp/features`.
- Do not expose server-only configuration to the browser unless the contract explicitly requires it.
- Let the webapp adapt to the API contract intentionally; do not silently “fix” backend semantics in frontend-only mapping code.

## Verification

After backend changes, run:

```bash
dotnet test backend/EventBoard.sln
```

After webapp changes, run:

```bash
npm run lint
```

from `webapp/`.

If a change spans both projects, run both checks.

## Documentation Follow-Through

If a task introduces or reveals a repo-specific technical decision, footgun, or guardrail, update the relevant doc in the same change unless there is a clear reason not to.

- Add durable architectural or implementation choices to `docs/technical-decisions.md`
- Add recurring pitfalls, sharp edges, or workflow constraints to `docs/footguns-and-guardrails.md`
- Do not mention a documentation gap in the close-out note without either updating the doc or explicitly explaining why no doc change was made

## Agent Expectations

- Inspect the owning layer before editing.
- Make the smallest change that matches the architecture.
- If a task requires breaking a documented boundary, call it out explicitly.
- Do not treat every existing pattern as ideal; use the deeper docs for decisions, risks, and exceptions.
- If behavior spans backend and webapp, change the layer that truly owns the behavior before adding compensating logic elsewhere.

## Required Close-Out Note

Before finishing a task, include a short close-out note that states:

- which instruction docs you followed
- one repo-specific constraint or guardrail you accounted for
- whether you updated `docs/technical-decisions.md` or `docs/footguns-and-guardrails.md`, and if not, why not
