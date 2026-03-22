using EventBoard.Application.DTOs;
using EventBoard.Application.Interfaces;
using EventBoard.Domain.Entities;

namespace EventBoard.Application.Services;

public class EventService(IEventRepository eventRepository, TimeProvider timeProvider) : IEventService
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private static readonly TimeSpan RegistrationDeadline = TimeSpan.FromHours(24); // 24 hours before the event starts, TODO: move this to env config

    public async Task<PagedResultDto<EventSummaryDto>> GetAllEventsAsync(
        EventListQueryDto query,
        CancellationToken ct = default)
    {
        var normalizedQuery = NormalizeQuery(query);
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        var eventsPage = await eventRepository.GetEventsPageAsync(normalizedQuery, ct);
        var mappedEventsDtos = MapEventsToSummaryDtos(eventsPage.Items, nowUtc);

        return new PagedResultDto<EventSummaryDto>(
            Items: mappedEventsDtos,
            Page: eventsPage.Page,
            PageSize: eventsPage.PageSize,
            TotalCount: eventsPage.TotalCount,
            TotalPages: eventsPage.TotalPages);
    }

    public async Task<EventDetailDto?> GetEventByIdAsync(int id, CancellationToken ct = default)
    {
        var eventDetail = await eventRepository.GetByIdAsync(id, ct);
        if (eventDetail is null) return null;

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var registrationDeadline = eventDetail.StartsAt.Subtract(RegistrationDeadline);

        return new EventDetailDto(
            Id: eventDetail.Id,
            Title: eventDetail.Title,
            Description: eventDetail.Description,
            StartsAt: eventDetail.StartsAt,
            Location: eventDetail.Location,
            MaxParticipants: eventDetail.MaxParticipants,
            CurrentRegistrations: eventDetail.CurrentRegistrations,
            IsUpcoming: IsUpcomingEvent(eventDetail.StartsAt, now),
            RegistrationOpen: IsRegistrationOpen(eventDetail.StartsAt, now, eventDetail.CurrentRegistrations, eventDetail.MaxParticipants),
            RegistrationDeadline: registrationDeadline
        );
    }

    private static IReadOnlyList<EventSummaryDto> MapEventsToSummaryDtos(IReadOnlyList<Event> events, DateTime nowUtc)
    {
        return [.. events.Select(e => new EventSummaryDto(
            Id: e.Id,
            Title: e.Title,
            StartsAt: e.StartsAt,
            Location: e.Location,
            MaxParticipants: e.MaxParticipants,
            CurrentRegistrations: e.CurrentRegistrations,
            IsUpcoming: IsUpcomingEvent(e.StartsAt, nowUtc),
            RegistrationOpen: IsRegistrationOpen(e.StartsAt, nowUtc, e.CurrentRegistrations, e.MaxParticipants)
        ))];
    }

    private static bool IsUpcomingEvent(DateTime startsAt, DateTime nowUtc)
    {
        return startsAt > nowUtc;
    }

    private static bool IsRegistrationOpen(DateTime startsAt, DateTime nowUtc, int currentRegistrations, int maxParticipants)
    {
        return startsAt.Subtract(RegistrationDeadline) > nowUtc && currentRegistrations < maxParticipants;
    }

    private static EventListQueryDto NormalizeQuery(EventListQueryDto query)
    {
        var page = query.Page < 1 ? DefaultPage : query.Page;
        var pageSize = query.PageSize < 1 ? DefaultPageSize : query.PageSize;
        if (pageSize > MaxPageSize) pageSize = MaxPageSize;

        return query with { Page = page, PageSize = pageSize };
    }
}
