using System.Text.Json.Serialization;

namespace EventBoard.Application.DTOs;

public record PagedResultDto<T>(
    [property: JsonPropertyName("items")]
    IReadOnlyList<T> Items,

    [property: JsonPropertyName("page")]
    int Page,

    [property: JsonPropertyName("page_size")]
    int PageSize,

    [property: JsonPropertyName("total_count")]
    int TotalCount,

    [property: JsonPropertyName("total_pages")]
    int TotalPages
);
