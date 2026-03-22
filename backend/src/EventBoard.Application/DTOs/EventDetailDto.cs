using System.Text.Json.Serialization;

namespace EventBoard.Application.DTOs;

public record EventDetailDto(
    [property: JsonPropertyName("id")]
    int Id,

    [property: JsonPropertyName("title")]
    string Title,

    [property: JsonPropertyName("description")]
    string Description,

    [property: JsonPropertyName("starts_at")]
    DateTime StartsAt,

    [property: JsonPropertyName("location")]
    string Location,

    [property: JsonPropertyName("max_participants")]
    int MaxParticipants,

    [property: JsonPropertyName("current_registrations")]
    int CurrentRegistrations,

    [property: JsonPropertyName("is_upcoming")]
    bool IsUpcoming,

    [property: JsonPropertyName("registration_open")]
    bool RegistrationOpen,

    [property: JsonPropertyName("registration_deadline")]
    DateTime RegistrationDeadline

);
