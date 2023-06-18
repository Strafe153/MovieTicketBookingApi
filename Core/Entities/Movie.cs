namespace Core.Entities;

public record Movie
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;

    public IList<MovieSession> MovieSessions { get; set; } = default!;
}
