# Technical Decisions

This file should capture only backend decisions that are important enough to guide future changes.

Add to it gradually when:

- a design choice is intentional and non-obvious
- future agents might otherwise “simplify” something incorrectly
- a recurring implementation pattern should be preserved

## Backend

### Layered backend structure

The backend is split into `Api`, `Application`, `Domain`, and `Infrastructure`.

Why it matters:

- controllers stay focused on HTTP
- business logic stays in services
- persistence stays in repositories

### Service-owned business rules

Business rules belong in the application layer, not controllers.

Current examples:

- `RegistrationService` owns registration rules
- `EventService` owns computed DTO flags like `IsUpcoming` and `RegistrationOpen`

Why it matters:

- behavior stays testable without HTTP
- controllers stay thin

### SQLite-backed persistence tests

Use SQLite-backed tests when EF behavior matters, especially around `ExecuteUpdateAsync`.

Why it matters:

- EF InMemory can hide real persistence behavior differences

### Transactional registration writes

The registration write path claims the seat and inserts the `Registration` row inside one repository-owned transaction.

Why it matters:

- the service still owns business-rule decisions
- the repository keeps EF transaction and constraint handling together
- `CurrentRegistrations` cannot drift when the insert fails after claiming a seat

### Registration race resolution happens at the database write boundary

The registration flow still performs service-level checks for event state, but last-seat races and concurrent duplicate emails are resolved by the transactional repository write.

Current behavior:

- the service validates event existence and whether registration is open
- the repository atomically increments `CurrentRegistrations` only when capacity remains
- the unique `(EventId, Email)` constraint is the final duplicate guard during concurrent writes

Why it matters:

- pre-write checks improve user-facing errors but are not authoritative under concurrency
- moving the winning/losing decision out of the transactional write path would reintroduce race conditions
- future cleanups should preserve the current split of responsibilities between service and repository

### SQLite is the required test substrate for registration persistence behavior

Registration persistence tests use SQLite in-memory instead of EF Core InMemory.

Why it matters:

- the registration flow depends on EF/SQLite behavior such as `ExecuteUpdateAsync`, transactions, and unique constraints
- EF Core InMemory can pass tests that would fail against the real persistence behavior
