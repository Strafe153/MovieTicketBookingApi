namespace Core.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public int SeatNumber { get; set; }

    public MovieSession MovieSession { get; set; } = default!;
    public Guid MovieSessionId { get; set; }

    public User User { get; set; } = default!;
    public Guid UserId { get; set; }
}
