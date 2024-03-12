namespace Domain.Entities;

public class MovieSession
{
	public Guid Id { get; set; }
	public DateTime DateTime { get; set; }
	public Guid MovieId { get; set; }
	public Guid MovieHallId { get; set; }
	public bool IsFinished { get; set; }
	public IList<Ticket> Tickets { get; set; } = default!;
}
