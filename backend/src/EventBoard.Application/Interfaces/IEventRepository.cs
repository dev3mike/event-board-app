using EventBoard.Application.DTOs;
using EventBoard.Domain.Entities;

namespace EventBoard.Application.Interfaces;

public interface IEventRepository
{
    Task<PagedResultDto<Event>> GetEventsPageAsync(
        EventListQueryDto query,
        CancellationToken ct = default);
    Task<Event?> GetByIdAsync(int id, CancellationToken ct = default);
}
