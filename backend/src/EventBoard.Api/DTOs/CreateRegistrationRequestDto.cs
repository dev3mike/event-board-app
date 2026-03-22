using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventBoard.Api.DTOs;

[Description("Registration payload for creating a new event registration.")]
public class CreateRegistrationRequestDto
{
    [Description("Full name of the participant.")]
    [Required]
    [StringLength(200, MinimumLength = 1)]
    [DefaultValue("John Doe")]
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [Description("Email address used for registration.")]
    [Required]
    [EmailAddress]
    [StringLength(320)]
    [DefaultValue("john.doe@example.com")]
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}
