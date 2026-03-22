using System.ComponentModel;
using System.Text.Json.Serialization;

namespace EventBoard.Api.DTOs;

[Description("Standard error payload returned by the API.")]
public class ApiErrorDto
{
    [Description("Machine-readable error code.")]
    [JsonPropertyName("code")]
    public string Code { get; init; } = "UnexpectedError";

    [Description("Human-readable error message.")]
    [JsonPropertyName("message")]
    public string Message { get; init; } = "An unexpected error occurred.";

    [Description("Field-level validation errors for bad requests.")]
    [JsonPropertyName("validation_errors")]
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
}
