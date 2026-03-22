using EventBoard.Application.Services;
using EventBoard.Application.Interfaces;
using EventBoard.Domain.Entities;
using EventBoard.Domain.Exceptions;
using EventBoard.Infrastructure.Persistence;
using EventBoard.Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace EventBoard.Tests.Application;

/// <summary>
/// Uses SQLite in-memory (not EF InMemory) so that ExecuteUpdateAsync is supported.
/// Each test gets its own connection to stay isolated.
/// </summary>
public class RegistrationServiceTests : IDisposable
{
    private static readonly DateTime FixedNowUtc = new(2026, 03, 21, 12, 00, 00, DateTimeKind.Utc);
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly TimeProvider _timeProvider;

    public RegistrationServiceTests()
    {
        // Keep connection open for the lifetime of the test — SQLite drops :memory: DBs when the last connection closes
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
        _timeProvider = new FixedTimeProvider(FixedNowUtc);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    private static Event MakeEvent(
        int maxParticipants = 10,
        int currentRegistrations = 0,
        DateTime? startsAt = null) => new()
        {
            Title = "Test Event",
            Description = "A test event.",
            Location = "Room 1",
            MaxParticipants = maxParticipants,
            CurrentRegistrations = currentRegistrations,
            StartsAt = startsAt ?? FixedNowUtc.AddDays(7)
        };

    private RegistrationService CreateService() =>
        new(
            new EventRepository(_db, _timeProvider),
            new RegistrationRepository(_db),
            _timeProvider,
            NullLogger<RegistrationService>.Instance);

    private RegistrationService CreateService(IRegistrationRepository registrationRepository) =>
        new(
            new EventRepository(_db, _timeProvider),
            registrationRepository,
            _timeProvider,
            NullLogger<RegistrationService>.Instance);

    [Fact]
    public async Task Register_WhenEventFull_ThrowsBusinessRuleException()
    {
        // given
        _db.Events.Add(MakeEvent(maxParticipants: 5, currentRegistrations: 5));
        await _db.SaveChangesAsync();

        // when
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().RegisterAsync(1, "Alice", "alice@example.com"));

        // then
        Assert.Equal("EventFull", ex.Code);
    }

    [Fact]
    public async Task Register_Within24HoursOfEvent_ThrowsBusinessRuleException()
    {
        // given
        _db.Events.Add(MakeEvent(startsAt: FixedNowUtc.AddHours(12)));
        await _db.SaveChangesAsync();

        // when
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().RegisterAsync(1, "Bob", "bob@example.com"));

        // then
        Assert.Equal("RegistrationClosed", ex.Code);
    }

    [Fact]
    public async Task Register_WithSameEmail_ThrowsBusinessRuleException()
    {
        // given
        _db.Events.Add(MakeEvent());
        await _db.SaveChangesAsync();

        var ev = _db.Events.Single();
        _db.Registrations.Add(new Registration
        {
            EventId = ev.Id,
            Name = "Alice",
            Email = "alice@example.com",
            RegisteredAt = FixedNowUtc
        });
        await _db.SaveChangesAsync();

        // when
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().RegisterAsync(ev.Id, "Alice2", "alice@example.com"));

        // then
        Assert.Equal("DuplicateRegistration", ex.Code);
    }

    [Fact]
    public async Task Register_WithSameEmailDifferentCasing_ThrowsBusinessRuleException()
    {
        // given
        _db.Events.Add(MakeEvent());
        await _db.SaveChangesAsync();

        var ev = _db.Events.Single();
        _db.Registrations.Add(new Registration
        {
            EventId = ev.Id,
            Name = "Alice",
            Email = "alice@example.com",
            RegisteredAt = FixedNowUtc
        });
        await _db.SaveChangesAsync();

        // when
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().RegisterAsync(ev.Id, "Alice2", "ALICE@EXAMPLE.COM"));

        // then
        Assert.Equal("DuplicateRegistration", ex.Code);
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsConfirmation()
    {
        // given
        _db.Events.Add(MakeEvent(startsAt: FixedNowUtc.AddDays(3)));
        await _db.SaveChangesAsync();

        var ev = _db.Events.Single();
        // when
        var confirmation = await CreateService().RegisterAsync(ev.Id, "Carol", "Carol@Example.com");

        // then
        Assert.Equal(ev.Title, confirmation.EventTitle);
        Assert.Equal(ev.StartsAt, confirmation.EventStartsAt);
        Assert.Equal("carol@example.com", confirmation.RegisteredEmail);
    }

    [Fact]
    public async Task Register_EmailPersistedAsNormalizedByRepository()
    {
        // given
        _db.Events.Add(MakeEvent());
        await _db.SaveChangesAsync();

        var ev = _db.Events.Single();
        // when
        await CreateService().RegisterAsync(ev.Id, "Dave", "Dave@Example.COM");

        // then
        var stored = _db.Registrations.Single();
        Assert.Equal("dave@example.com", stored.Email);
    }

    [Fact]
    public async Task Register_ForNonExistentEvent_ThrowsNotFoundException()
    {
        // when + then
        await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateService().RegisterAsync(999, "Eve", "eve@example.com"));
    }

    [Fact]
    public async Task Register_WhenTransactionalWriteDetectsDuplicate_ThrowsBusinessRuleException()
    {
        // given
        _db.Events.Add(MakeEvent());
        await _db.SaveChangesAsync();

        var ev = _db.Events.Single();
        var duplicateRepository = new StubRegistrationRepository(RegistrationWriteResult.DuplicateRegistration());

        // when
        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService(duplicateRepository).RegisterAsync(ev.Id, "Eve", "eve@example.com"));

        // then
        Assert.Equal("DuplicateRegistration", ex.Code);
    }

    private sealed class StubRegistrationRepository(RegistrationWriteResult writeResult) : IRegistrationRepository
    {
        public Task<bool> ExistsForEventAsync(int eventId, string email, CancellationToken ct = default) =>
            Task.FromResult(false);

        public Task<RegistrationWriteResult> TryCreateForEventAsync(Registration registration, CancellationToken ct = default) =>
            Task.FromResult(writeResult);
    }

    private sealed class FixedTimeProvider(DateTime utcNow) : TimeProvider
    {
        private readonly DateTimeOffset _utcNow = new(DateTime.SpecifyKind(utcNow, DateTimeKind.Utc));

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }
}
