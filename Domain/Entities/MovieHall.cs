namespace Domain.Entities;

public class MovieHall
{
	public Guid Id { get; set; }
	public string Name { get; set; } = default!;
	public int NumberOfSeats { get; set; }
	public bool IsActive { get; set; } = true;
	public IList<MovieSession> MovieSessions { get; set; } = default!;
}
