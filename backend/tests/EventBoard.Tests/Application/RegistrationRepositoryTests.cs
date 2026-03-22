using EventBoard.Application.Interfaces;
using EventBoard.Domain.Entities;
using EventBoard.Infrastructure.Persistence;
using EventBoard.Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EventBoard.Tests.Application;

public class RegistrationRepositoryTests : IDisposable
{
    private static readonly DateTime FixedNowUtc = new(2026, 03, 21, 12, 00, 00, DateTimeKind.Utc);
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly RegistrationRepository _repository;

    public RegistrationRepositoryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
        _repository = new RegistrationRepository(_db);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task ExistsForEventAsync_IsCaseInsensitiveAgainstStoredEmail()
    {
        // given
        var ev = new Event
        {
            Title = "Test Event",
            Description = "A test event.",
            Location = "Room 1",
            MaxParticipants = 10,
            CurrentRegistrations = 0,
            StartsAt = FixedNowUtc.AddDays(7)
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();

        _db.Registrations.Add(new Registration
        {
            EventId = ev.Id,
            Name = "Alice",
            Email = "alice@example.com",
            RegisteredAt = FixedNowUtc
        });
        await _db.SaveChangesAsync();

        // when + then
        Assert.True(await _repository.ExistsForEventAsync(ev.Id, "ALICE@EXAMPLE.COM"));
    }

    [Fact]
    public async Task TryCreateForEventAsync_PersistsEmailNormalized()
    {
        // given
        var ev = new Event
        {
            Title = "Test Event",
            Description = "A test event.",
            Location = "Room 1",
            MaxParticipants = 10,
            CurrentRegistrations = 0,
            StartsAt = FixedNowUtc.AddDays(7)
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();

        // when
        var result = await _repository.TryCreateForEventAsync(new Registration
        {
            EventId = ev.Id,
            Name = "Bob",
            Email = "  Bob@Example.COM  ",
            RegisteredAt = FixedNowUtc
        });

        // then
        Assert.Equal(RegistrationWriteStatus.Success, result.Status);
        Assert.NotNull(result.Registration);
        Assert.Equal("bob@example.com", result.Registration!.Email);

        var persisted = await _db.Registrations.SingleAsync();
        Assert.Equal("bob@example.com", persisted.Email);
    }

    [Fact]
    public async Task TryCreateForEventAsync_WhenDuplicateInsertFails_RollsBackRegistrationCount()
    {
        // given
        var ev = new Event
        {
            Title = "Test Event",
            Description = "A test event.",
            Location = "Room 1",
            MaxParticipants = 10,
            CurrentRegistrations = 0,
            StartsAt = FixedNowUtc.AddDays(7)
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();

        _db.Registrations.Add(new Registration
        {
            EventId = ev.Id,
            Name = "Alice",
            Email = "alice@example.com",
            RegisteredAt = FixedNowUtc
        });
        await _db.SaveChangesAsync();

        // when
        var result = await _repository.TryCreateForEventAsync(new Registration
        {
            EventId = ev.Id,
            Name = "Alice Again",
            Email = "alice@example.com",
            RegisteredAt = FixedNowUtc
        });

        // then
        Assert.Equal(RegistrationWriteStatus.DuplicateRegistration, result.Status);
        Assert.Null(result.Registration);

        var persistedEvent = await _db.Events.SingleAsync();
        Assert.Equal(0, persistedEvent.CurrentRegistrations);
        Assert.Equal(1, await _db.Registrations.CountAsync());
    }
}
