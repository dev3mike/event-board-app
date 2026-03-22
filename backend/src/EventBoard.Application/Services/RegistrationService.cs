using EventBoard.Application.DTOs;
using EventBoard.Application.Interfaces;
using EventBoard.Domain.Entities;
using EventBoard.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace EventBoard.Application.Services;

public class RegistrationService(
    IEventRepository eventRepository,
    IRegistrationRepository registrationRepository,
    TimeProvider timeProvider,
    ILogger<RegistrationService> logger) : IRegistrationService
{
    private static readonly TimeSpan RegistrationDeadline = TimeSpan.FromHours(24); // 24 hours before the event starts, TODO: move this to env config

    public async Task<RegistrationConfirmationDto> RegisterAsync(
        int eventId,
        string name,
        string email,
        CancellationToken ct = default)
    {
        var eventDetail = await eventRepository.GetByIdAsync(eventId, ct)
            ?? throw new NotFoundException($"Event {eventId} not found.");

        var now = timeProvider.GetUtcNow().UtcDateTime;

        AssertRegistrationOpen(eventDetail, now);
        AssertEventHasCapacity(eventDetail);
        await AssertEmailNotAlreadyRegisteredAsync(eventId, email, ct);

        var registration = new Registration
        {
            EventId = eventId,
            Name = name.Trim(),
            Email = email,
            RegisteredAt = now
        };

        var writeResult = await registrationRepository.TryCreateForEventAsync(registration, ct);
        var saved = AssertRegistrationWritten(writeResult);

        logger.LogInformation("Registration created successfully. EventId={EventId}, RegistrationId={RegistrationId}", eventId, saved.Id);

        return new RegistrationConfirmationDto(
            saved.Id,
            eventDetail.Title,
            eventDetail.StartsAt,
            eventDetail.Location,
            saved.Name,
            saved.Email);
    }

    private static void AssertRegistrationOpen(Event eventDetail, DateTime now)
    {
        if (now >= eventDetail.StartsAt - RegistrationDeadline)
            throw new BusinessRuleException(
                "RegistrationClosed",
                "Registration has closed. The deadline is 24 hours before the event starts.");
    }

    private static void AssertEventHasCapacity(Event eventDetail)
    {
        if (eventDetail.CurrentRegistrations >= eventDetail.MaxParticipants)
            throw new BusinessRuleException(
                "EventFull",
                "This event has reached its maximum number of participants.");
    }

    private async Task AssertEmailNotAlreadyRegisteredAsync(
        int eventId,
        string email,
        CancellationToken ct)
    {
        if (await registrationRepository.ExistsForEventAsync(eventId, email, ct))
            throw new BusinessRuleException(
                "DuplicateRegistration",
                "This email address is already registered for this event.");
    }

    private static Registration AssertRegistrationWritten(RegistrationWriteResult writeResult)
    {
        return writeResult.Status switch
        {
            RegistrationWriteStatus.Success when writeResult.Registration is not null => writeResult.Registration,
            RegistrationWriteStatus.EventFull => throw new BusinessRuleException(
                "EventFull",
                "This event just filled up. No spots remain."),
            RegistrationWriteStatus.DuplicateRegistration => throw new BusinessRuleException(
                "DuplicateRegistration",
                "This email address is already registered for this event."),
            _ => throw new InvalidOperationException("Registration write result was incomplete.")
        };
    }
}
