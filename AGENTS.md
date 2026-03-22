# AGENTS.md

## Mission

Change the layer that owns the behavior, preserve the API contract unless intentionally changing it, and leave clear tests behind.

## Read Before Editing

- [CLAUDE.md](/CLAUDE.md)
- [docs/technical-decisions.md](/docs/technical-decisions.md)
- [docs/footguns-and-guardrails.md](/docs/footguns-and-guardrails.md)

## Ownership Rules

- `backend/src/EventBoard.Api`
  - controllers, middleware, startup, transport concerns
- `backend/src/EventBoard.Application`
  - business rules, orchestration, DTO shaping, repository contracts
- `backend/src/EventBoard.Domain`
  - entities, exceptions, shared helpers
- `backend/src/EventBoard.Infrastructure`
  - EF Core, repository implementations, migrations, seeding

## Do

- Keep controllers thin.
- Route business behavior through services.
- Keep persistence mechanics in repositories.
- Use `TimeProvider` for time-based logic.
- Thread `CancellationToken` through async calls.
- Make API contract changes explicit.
- Add or update tests with backend behavior changes.

## Do Not

- Do not bypass services from controllers.
- Do not put HTTP concerns in `Application` or `Domain`.
- Do not return EF entities directly from controllers.
- Do not silently change error codes, JSON casing, or response shapes.
- Do not copy low-signal patterns like unused injected dependencies.

## Verify

```bash
dotnet test backend/EventBoard.sln
```

## Required Final Note

Before finishing, state:

- which docs you followed
- one repo-specific guardrail or constraint you respected
- whether this task should add anything to `docs/technical-decisions.md` or `docs/footguns-and-guardrails.md`
