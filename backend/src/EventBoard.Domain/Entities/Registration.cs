namespace EventBoard.Domain.Entities;

public class Registration
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public DateTime RegisteredAt { get; set; }

    public Event Event { get; set; } = null!;
}
