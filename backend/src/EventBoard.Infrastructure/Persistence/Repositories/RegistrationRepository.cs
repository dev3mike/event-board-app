using EventBoard.Application.Interfaces;
using EventBoard.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EventBoard.Infrastructure.Persistence.Repositories;

public class RegistrationRepository(AppDbContext db) : IRegistrationRepository
{
    public async Task<bool> ExistsForEventAsync(int eventId, string email, CancellationToken ct = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        return await db.Registrations
            .AnyAsync(r => r.EventId == eventId && r.Email == normalizedEmail, ct);
    }

    public async Task<RegistrationWriteResult> TryCreateForEventAsync(Registration registration, CancellationToken ct = default)
    {
        registration.Email = NormalizeEmail(registration.Email);

        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        // Claim a seat only if capacity still exists, so concurrent last-spot requests produce one winner.
        int rowsAffected = await db.Events
            .Where(e => e.Id == registration.EventId && e.CurrentRegistrations < e.MaxParticipants)
            .ExecuteUpdateAsync(
                s => s.SetProperty(e => e.CurrentRegistrations, e => e.CurrentRegistrations + 1),
                ct);

        if (rowsAffected == 0)
        {
            await transaction.RollbackAsync(ct);
            return RegistrationWriteResult.EventFull();
        }

        try
        {
            // Persist the registration inside the same transaction to keep the counter and row in sync.
            db.Registrations.Add(registration);
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return RegistrationWriteResult.Succeeded(registration);
        }
        catch (DbUpdateException ex) when (IsUniqueEmailConstraintViolation(ex))
        {
            // The unique email constraint is the final duplicate guard under concurrent requests.
            db.ChangeTracker.Clear();
            await transaction.RollbackAsync(ct);
            return RegistrationWriteResult.DuplicateRegistration();
        }
    }

    private static bool IsUniqueEmailConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is SqliteException sqliteEx
            && sqliteEx.SqliteErrorCode == 19;
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
