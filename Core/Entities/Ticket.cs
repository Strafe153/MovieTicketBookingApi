namespace Core.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public int SeatNumber { get; set; }
    public Guid MovieSessionId { get; set; }
    public Guid UserId { get; set; }
    public bool IsCompleted { get; set; }
}
