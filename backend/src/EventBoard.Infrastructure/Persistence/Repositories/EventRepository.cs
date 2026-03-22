using EventBoard.Application.DTOs;
using EventBoard.Application.Interfaces;
using EventBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventBoard.Infrastructure.Persistence.Repositories;

public class EventRepository(AppDbContext db, TimeProvider timeProvider) : IEventRepository
{
    public async Task<PagedResultDto<Event>> GetEventsPageAsync(
        EventListQueryDto query,
        CancellationToken ct = default)
    {
        IQueryable<Event> eventsQuery = db.Events.AsNoTracking();
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;

        if (query.UpcomingOnly)
        {
            eventsQuery = eventsQuery.Where(e => e.StartsAt > nowUtc);
        }

        var totalCount = await eventsQuery.CountAsync(ct);
        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)query.PageSize);

        var items = await eventsQuery
            .OrderBy(e => e.StartsAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return new PagedResultDto<Event>(items, query.Page, query.PageSize, totalCount, totalPages);
    }

    public async Task<Event?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await db.Events.FindAsync([id], ct);
    }
}
