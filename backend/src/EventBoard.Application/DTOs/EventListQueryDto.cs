using System.Text.Json.Serialization;

namespace EventBoard.Application.DTOs;

public record EventListQueryDto(
    [property: JsonPropertyName("upcoming_only")]
    bool UpcomingOnly = false,

    [property: JsonPropertyName("page")]
    int Page = 1,

    [property: JsonPropertyName("page_size")]
    int PageSize = 20

);
