using EventBoard.Application.DTOs;

namespace EventBoard.Application.Interfaces;

public interface IEventService
{
    Task<PagedResultDto<EventSummaryDto>> GetAllEventsAsync(EventListQueryDto query, CancellationToken ct = default);
    Task<EventDetailDto?> GetEventByIdAsync(int id, CancellationToken ct = default);
}
