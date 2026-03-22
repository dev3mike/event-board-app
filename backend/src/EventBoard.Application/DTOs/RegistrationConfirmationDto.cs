using System.Text.Json.Serialization;

namespace EventBoard.Application.DTOs;

public record RegistrationConfirmationDto(
    [property: JsonPropertyName("registration_id")]
    int RegistrationId,

    [property: JsonPropertyName("event_title")]
    string EventTitle,

    [property: JsonPropertyName("event_starts_at")]
    DateTime EventStartsAt,

    [property: JsonPropertyName("event_location")]
    string EventLocation,

    [property: JsonPropertyName("registered_name")]
    string RegisteredName,

    [property: JsonPropertyName("registered_email")]
    string RegisteredEmail

);
