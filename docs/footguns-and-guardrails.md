# Footguns and Guardrails

Add only high-signal risks that future agents should see before changing backend behavior.

Add to it gradually when:

- a bug risk is subtle
- a cleanup could easily introduce regressions
- a boundary is easy to blur accidentally

## Backend

### Registration seat claim and insert must commit together

The registration flow relies on an atomic seat-claim update plus a unique `(EventId, Email)` index.

Guardrail:

- do not split the seat claim and registration insert across separate commits
- do not treat the pre-insert duplicate check as the authoritative duplicate guard
- do not replace the conditional seat claim with a read-then-write capacity check in the service

Why it matters:

- a later insert failure can drift `CurrentRegistrations`
- concurrent duplicate requests must be resolved by the database constraint and transaction rollback
- concurrent last-seat requests need the database update itself to decide the winner

### Service vs repository ownership

Repositories should own EF mechanics. Services should own business rules and computed behavior.

Guardrail:

- do not move `IsUpcoming` or `RegistrationOpen` into controllers or repositories as a casual cleanup
- do not move transaction orchestration or uniqueness/constraint translation out of `RegistrationRepository` just because the service already performs pre-checks

### Registration concurrency tests need real relational behavior

The registration path depends on relational database features, not just in-memory collection semantics.

Guardrail:

- when changing registration persistence behavior, keep or add SQLite-backed tests
- do not switch these tests to EF Core InMemory for convenience

### API contract stability

The current API uses snake_case JSON, `ApiErrorDto`, and `409 Conflict` for business-rule failures.

Guardrail:

- do not casually change field names, error codes, or status semantics

### Startup migration and seeding behavior

`Program.cs` currently runs migrations and seeding on startup.

Guardrail:

- treat this as current project behavior, not a universal pattern to copy elsewhere

## Webapp

### Do not replace `updateTag` with `revalidateTag` in Server Actions

The registration Server Action uses `updateTag(tag)` for cache invalidation.

Guardrail:

- do not swap `updateTag` for `revalidateTag(tag)` — the single-argument form is deprecated in Next.js 16
- do not call `revalidateTag(tag, 'max')` from a Server Action either — that form is intended for Route Handlers
- `updateTag` is the only correct cache invalidation API inside a Server Action in this version

### Do not throw from Server Actions that use `useActionState`

The `registerForEvent` Server Action returns a `RegistrationActionResult` and never throws.

Guardrail:

- do not refactor the catch block to re-throw errors
- throwing from a `useActionState`-backed Server Action escalates to the nearest error boundary, destroying form state and giving the user no recovery path for expected errors like `EventFull` or `DuplicateRegistration`

### Registration success state must live above RSC-driven `registration_open`

After a successful registration, `updateTag` refreshes the event; when the last seat is taken, `registration_open` becomes false and the server can re-render before a child `useEffect` runs.

Guardrail:

- keep `useActionState` for registration in a client parent (e.g. `EventRegistrationSection`) and branch on `state.status === 'success'` before branching on `registration_open`
- do not rely on a child-only callback to lift success into the parent when the parent can swap the form for a “closed” banner on the same navigation

### `API_BASE_URL` must not gain a `NEXT_PUBLIC_` prefix

The backend URL is read only by Server Components and Server Actions.

Guardrail:

- do not add `NEXT_PUBLIC_` to `API_BASE_URL`
- doing so exposes the backend origin in the browser bundle; there is no legitimate reason to call the API from the client side in this architecture
