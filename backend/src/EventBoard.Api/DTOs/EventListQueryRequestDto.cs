using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace EventBoard.Api.DTOs;

[Description("Query parameters for listing events.")]
public class EventListQueryRequestDto
{
    [DefaultValue(false)]
    [FromQuery(Name = "upcoming_only")]
    [JsonPropertyName("upcoming_only")]
    public bool UpcomingOnly { get; set; } = false;

    [Range(1, int.MaxValue)]
    [DefaultValue(1)]
    [FromQuery(Name = "page")]
    [JsonPropertyName("page")]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    [DefaultValue(20)]
    [FromQuery(Name = "page_size")]
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; } = 20;
}
