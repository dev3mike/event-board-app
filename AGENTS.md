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
- `webapp/app`
  - routes, layouts, loading/error boundaries, page-level server-rendered composition
- `webapp/features`
  - feature queries, Server Actions, feature types, feature UI
- `webapp/components`
  - shared presentational UI primitives
- `webapp/lib`
  - shared API client code and cross-feature utilities

## Do

- Keep controllers thin.
- Route business behavior through services.
- Keep persistence mechanics in repositories.
- Keep route composition in `webapp/app`.
- Keep feature-specific fetching, actions, and state transitions in `webapp/features`.
- Keep shared components generic and presentation-focused.
- Use `TimeProvider` for time-based logic.
- Thread `CancellationToken` through async calls.
- Make API contract changes explicit.
- Add or update tests with backend behavior changes.
- Run frontend verification when changing webapp behavior.

## Do Not

- Do not bypass services from controllers.
- Do not put HTTP concerns in `Application` or `Domain`.
- Do not return EF entities directly from controllers.
- Do not silently change error codes, JSON casing, or response shapes.
- Do not copy low-signal patterns like unused injected dependencies.
- Do not let `webapp/components` accumulate feature-specific business logic.
- Do not move backend contract interpretation into ad hoc client-side fetch code when it belongs in shared server-side API helpers or feature modules.
- Do not silently paper over backend contract mismatches in the UI; fix the owning layer or make the contract change explicit.

## Verify

```bash
dotnet test backend/EventBoard.sln
```

For `webapp/` changes, also run:

```bash
npm run lint
```

from `webapp/`.

If the task touches both projects, run both checks.

## Documentation Follow-Through

If a task introduces or reveals a repo-specific technical decision, footgun, or guardrail, update the relevant doc in the same change unless there is a clear reason not to.

- Add durable architectural or implementation choices to `docs/technical-decisions.md`
- Add recurring pitfalls, sharp edges, or workflow constraints to `docs/footguns-and-guardrails.md`
- Do not mention a documentation gap in the final note without either updating the doc or explicitly explaining why no doc change was made

## Required Final Note

Before finishing, state:

- which docs you followed
- one repo-specific guardrail or constraint you respected
- whether you updated `docs/technical-decisions.md` or `docs/footguns-and-guardrails.md`, and if not, why not
