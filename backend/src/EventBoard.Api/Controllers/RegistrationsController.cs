using EventBoard.Api.DTOs;
using EventBoard.Application.DTOs;
using EventBoard.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace EventBoard.Api.Controllers;

[ApiController]
[Route("api/events/{eventId:int}/registrations")]
[Produces("application/json")]
public class RegistrationsController(IRegistrationService registrationService) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Register for event")]
    [ProducesResponseType(typeof(RegistrationConfirmationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromRoute][Range(1, int.MaxValue)] int eventId,
        [FromBody] CreateRegistrationRequestDto request,
        CancellationToken ct)
    {
        var confirmation = await registrationService.RegisterAsync(eventId, request.Name, request.Email.Trim(), ct);
        return CreatedAtAction(nameof(Register), new { eventId }, confirmation);
    }
}
