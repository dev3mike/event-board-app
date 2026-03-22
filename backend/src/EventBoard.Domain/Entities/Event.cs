namespace EventBoard.Domain.Entities;

public class Event
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime StartsAt { get; set; }
    public required string Location { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentRegistrations { get; set; }

    public ICollection<Registration> Registrations { get; set; } = [];
}
