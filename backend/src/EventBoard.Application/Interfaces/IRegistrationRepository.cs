using EventBoard.Domain.Entities;

namespace EventBoard.Application.Interfaces;

public enum RegistrationWriteStatus
{
    Success,
    EventFull,
    DuplicateRegistration
}

public sealed record RegistrationWriteResult(RegistrationWriteStatus Status, Registration? Registration)
{
    public static RegistrationWriteResult Succeeded(Registration registration) =>
        new(RegistrationWriteStatus.Success, registration);

    public static RegistrationWriteResult EventFull() =>
        new(RegistrationWriteStatus.EventFull, null);

    public static RegistrationWriteResult DuplicateRegistration() =>
        new(RegistrationWriteStatus.DuplicateRegistration, null);
}

public interface IRegistrationRepository
{
    Task<bool> ExistsForEventAsync(int eventId, string email, CancellationToken ct = default);
    Task<RegistrationWriteResult> TryCreateForEventAsync(Registration registration, CancellationToken ct = default);
}
