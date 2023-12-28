using Core.Enums;

namespace Core.Entities;

public record Movie
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public int DurationInMinutes { get; set; }
    public AgeRating AgeRating { get; set; }
    public IList<MovieSession> MovieSessions { get; set; } = default!;
}
