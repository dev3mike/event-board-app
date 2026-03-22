using EventBoard.Api.DTOs;
using EventBoard.Application.DTOs;
using EventBoard.Application.Interfaces;
using EventBoard.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EventBoard.Api.Controllers;

[ApiController]
[Route("api/events")]
[Produces("application/json")]
public class EventsController(IEventService eventService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "List events")]
    [ProducesResponseType(typeof(PagedResultDto<EventSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] EventListQueryRequestDto request,
        CancellationToken ct = default)
    {
        var query = new EventListQueryDto(request.UpcomingOnly, request.Page, request.PageSize);
        var events = await eventService.GetAllEventsAsync(query, ct);
        return Ok(events);
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get event by id")]
    [ProducesResponseType(typeof(EventDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute][Range(1, int.MaxValue)] int id, CancellationToken ct)
    {
        var eventDetail = await eventService.GetEventByIdAsync(id, ct);

        if (eventDetail is null)
            throw new NotFoundException($"Event {id} not found.");

        return Ok(eventDetail);
    }
}
