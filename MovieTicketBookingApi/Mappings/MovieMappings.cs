using MovieTicketBookingApi.Protos.V1.Movies;
using CoreMovie = Domain.Entities.Movie;
using CoreAgeRating = Domain.Enums.AgeRating;

namespace MovieTicketBookingApi.Mappings;

public static class MovieMappings
{
    public static Movie ToMovie(this CoreMovie coreMovie)
    {
        Movie movie = new()
        {
            Id = coreMovie.Id.ToString(),
            Title = coreMovie.Title,
            DurationInMinutes = coreMovie.DurationInMinutes,
            AgeRating = (AgeRating)coreMovie.AgeRating
        };

        if (coreMovie.MovieSessions is not null)
        {
            movie.MovieSessions.AddRange(coreMovie.MovieSessions.Select(MovieSessionMappings.ToSession));
        }

        return movie;
    }

    public static GetAllMoviesReply ToReply(this IList<CoreMovie> list)
    {
        GetAllMoviesReply reply = new();
        reply.Movies.AddRange(list.Select(ToMovie));

        return reply;
    }

    public static GetMovieByIdReply ToGetByIdReply(this CoreMovie movie)
    {
        GetMovieByIdReply reply = new()
        {
            Id = movie.Id.ToString(),
            Title = movie.Title,
            DurationInMinutes = movie.DurationInMinutes,
            AgeRating = (AgeRating)movie.AgeRating
        };

        if (movie.MovieSessions is not null)
        {
            reply.MovieSessions.AddRange(movie.MovieSessions.Select(MovieSessionMappings.ToSession));
        }

        return reply;
    }

    public static CoreMovie ToMovie(this CreateMovieRequest request) =>
        new()
        {
            Title = request.Title,
            DurationInMinutes = request.DurationInMinutes,
            AgeRating = (CoreAgeRating)request.AgeRating
        };

    public static CreateMovieReply ToCreateReply(this CoreMovie movie) => new()
    {
        Id = movie.Id.ToString(),
        Title = movie.Title,
        DurationInMinutes = movie.DurationInMinutes,
        AgeRating = (AgeRating)movie.AgeRating
    };

    public static void Update(this UpdateMovieRequest request, CoreMovie movie)
    {
        movie.Title = request.Title;
        movie.DurationInMinutes = request.DurationInMinutes;
        movie.AgeRating = (CoreAgeRating)request.AgeRating;
    }
}