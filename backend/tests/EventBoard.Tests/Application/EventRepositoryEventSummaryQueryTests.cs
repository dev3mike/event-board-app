using EventBoard.Application.DTOs;
using EventBoard.Domain.Entities;
using EventBoard.Infrastructure.Persistence;
using EventBoard.Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EventBoard.Tests.Application;

public class EventRepositoryEventSummaryQueryTests : IDisposable
{
    private static readonly DateTime FixedNowUtc = new(2026, 03, 21, 12, 00, 00, DateTimeKind.Utc);
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly EventRepository _repository;

    public EventRepositoryEventSummaryQueryTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
        _repository = new EventRepository(_db, new FixedTimeProvider(FixedNowUtc));
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task GetEventsPageAsync_AppliesPagingAndReportsMetadata()
    {
        // given
        await SeedEventsAsync(
            MakeEvent("E1", FixedNowUtc.AddHours(1), 10, 0),
            MakeEvent("E2", FixedNowUtc.AddHours(2), 10, 0),
            MakeEvent("E3", FixedNowUtc.AddHours(3), 10, 0));

        // when
        var result = await _repository.GetEventsPageAsync(new EventListQueryDto(false, 1, 2));

        // then
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(["E1", "E2"], result.Items.Select(i => i.Title).ToArray());
    }

    [Fact]
    public async Task GetEventsPageAsync_WhenUpcomingOnly_ReturnsOnlyFutureEvents()
    {
        // given
        await SeedEventsAsync(
            MakeEvent("Past", FixedNowUtc.AddHours(-1), 10, 0),
            MakeEvent("Future", FixedNowUtc.AddHours(5), 10, 0));

        // when
        var result = await _repository.GetEventsPageAsync(new EventListQueryDto(true, 1, 20));

        // then
        Assert.Single(result.Items);
        Assert.Equal("Future", result.Items[0].Title);
    }

    [Fact]
    public async Task GetEventsPageAsync_OrdersDeterministicallyAcrossPages()
    {
        // given
        await SeedEventsAsync(
            MakeEvent("Late", FixedNowUtc.AddHours(5), 10, 0),
            MakeEvent("Early", FixedNowUtc.AddHours(1), 10, 0),
            MakeEvent("Middle", FixedNowUtc.AddHours(3), 10, 0));

        // when
        var page1 = await _repository.GetEventsPageAsync(new EventListQueryDto(false, 1, 2));
        var page2 = await _repository.GetEventsPageAsync(new EventListQueryDto(false, 2, 2));

        // then
        Assert.Equal(["Early", "Middle"], page1.Items.Select(i => i.Title).ToArray());
        Assert.Single(page2.Items);
        Assert.Equal("Late", page2.Items[0].Title);
    }

    [Fact]
    public async Task GetEventsPageAsync_DoesNotComputeDtoFlagsInRepository()
    {
        // given
        await SeedEventsAsync(MakeEvent("Any", FixedNowUtc.AddHours(25), 10, 5));

        // when
        var result = await _repository.GetEventsPageAsync(new EventListQueryDto(false, 1, 20));

        // then
        Assert.Single(result.Items);
        Assert.Equal("Any", result.Items[0].Title);
    }

    private async Task SeedEventsAsync(params Event[] events)
    {
        _db.Events.AddRange(events);
        await _db.SaveChangesAsync();
    }

    private static Event MakeEvent(string title, DateTime startsAt, int maxParticipants, int currentRegistrations) => new()
    {
        Title = title,
        Description = $"{title} description",
        StartsAt = startsAt,
        Location = "Test hall",
        MaxParticipants = maxParticipants,
        CurrentRegistrations = currentRegistrations
    };

    private sealed class FixedTimeProvider(DateTime utcNow) : TimeProvider
    {
        private readonly DateTimeOffset _utcNow = new(DateTime.SpecifyKind(utcNow, DateTimeKind.Utc));

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }
}
