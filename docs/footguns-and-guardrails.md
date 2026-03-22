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
