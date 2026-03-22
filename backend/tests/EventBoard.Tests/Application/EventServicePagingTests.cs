using EventBoard.Application.DTOs;
using EventBoard.Application.Interfaces;
using EventBoard.Application.Services;
using EventBoard.Domain.Entities;

namespace EventBoard.Tests.Application;

public class EventServicePagingTests
{
    private static readonly DateTime FixedNowUtc = new(2026, 03, 21, 12, 00, 00, DateTimeKind.Utc);

    [Fact]
    public async Task GetAllEventsAsync_NormalizesInvalidPaginationValues()
    {
        // given
        var repo = new CapturingEventRepository();
        var sut = new EventService(repo, new FixedTimeProvider(FixedNowUtc));

        // when
        await sut.GetAllEventsAsync(new EventListQueryDto(false, 0, 0));

        // then
        Assert.NotNull(repo.ReceivedQuery);
        Assert.Equal(1, repo.ReceivedQuery!.Page);
        Assert.Equal(20, repo.ReceivedQuery.PageSize);
    }

    [Fact]
    public async Task GetAllEventsAsync_CapsPageSizeAt100()
    {
        // given
        var repo = new CapturingEventRepository();
        var sut = new EventService(repo, new FixedTimeProvider(FixedNowUtc));

        // when
        await sut.GetAllEventsAsync(new EventListQueryDto(false, 1, 1000));

        // then
        Assert.NotNull(repo.ReceivedQuery);
        Assert.Equal(100, repo.ReceivedQuery!.PageSize);
    }

    [Fact]
    public async Task GetAllEventsAsync_MapsDtoFlagsInService()
    {
        // given
        var now = FixedNowUtc;
        var repo = new CapturingEventRepository
        {
            NextPage = new PagedResultDto<Event>(
            [
                new Event
                {
                    Id = 1,
                    Title = "Open",
                    Description = "desc",
                    StartsAt = now.AddHours(25),
                    Location = "Room",
                    MaxParticipants = 10,
                    CurrentRegistrations = 5
                },
                new Event
                {
                    Id = 2,
                    Title = "ClosedByTime",
                    Description = "desc",
                    StartsAt = now.AddHours(23),
                    Location = "Room",
                    MaxParticipants = 10,
                    CurrentRegistrations = 1
                }
            ],
            1,
            20,
            2,
            1)
        };

        var sut = new EventService(repo, new FixedTimeProvider(now));
        // when
        var result = await sut.GetAllEventsAsync(new EventListQueryDto(false, 1, 20));

        // then
        Assert.Equal(2, result.Items.Count);
        Assert.True(result.Items.Single(x => x.Title == "Open").RegistrationOpen);
        Assert.False(result.Items.Single(x => x.Title == "ClosedByTime").RegistrationOpen);
    }

    private sealed class CapturingEventRepository : IEventRepository
    {
        public EventListQueryDto? ReceivedQuery { get; private set; }
        public PagedResultDto<Event>? NextPage { get; init; }

        public Task<PagedResultDto<Event>> GetEventsPageAsync(
            EventListQueryDto query,
            CancellationToken ct = default)
        {
            ReceivedQuery = query;
            return Task.FromResult(NextPage ?? new PagedResultDto<Event>([], query.Page, query.PageSize, 0, 0));
        }

        public Task<Event?> GetByIdAsync(int id, CancellationToken ct = default) =>
            Task.FromResult<Event?>(null);
    }

    private sealed class FixedTimeProvider(DateTime utcNow) : TimeProvider
    {
        private readonly DateTimeOffset _utcNow = new(DateTime.SpecifyKind(utcNow, DateTimeKind.Utc));

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }
}
