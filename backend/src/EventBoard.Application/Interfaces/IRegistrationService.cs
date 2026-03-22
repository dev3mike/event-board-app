using EventBoard.Application.DTOs;

namespace EventBoard.Application.Interfaces;

public interface IRegistrationService
{
    Task<RegistrationConfirmationDto> RegisterAsync(
        int eventId,
        string name,
        string email,
        CancellationToken ct = default);
}
