namespace Core.Entities;

public class MovieSession
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }

    public Movie Movie { get; set; } = default!;
    public Guid MovieId { get; set; }

    public MovieHall MovieHall { get; set; } = default!;
    public Guid MovieHallId { get; set; }

    public IList<Ticket> Tickets { get; set; } = default!;
}
