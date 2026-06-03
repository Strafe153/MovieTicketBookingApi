using MovieTicketBookingApi.Protos.V1.MovieHalls;
using CoreMovieHall = Domain.Entities.MovieHall;

namespace MovieTicketBookingApi.Mappings;

public static class MovieHallMappings
{
    public static MovieHall ToHall(this CoreMovieHall movieHall)
    {
        MovieHall hall = new()
        {
            Id = movieHall.Id.ToString(),
            Name = movieHall.Name,
            NumberOfSeats = movieHall.NumberOfSeats,
        };

        if (movieHall.MovieSessions is not null)
        {
            hall.MovieSessions.AddRange(movieHall.MovieSessions.Select(MovieSessionMappings.ToSession));
        }

        return hall;
    }

    public static GetAllMovieHallsReply ToReply(this IList<CoreMovieHall> list)
    {
        GetAllMovieHallsReply reply = new();
        reply.MovieHalls.AddRange(list.Select(ToHall));

        return reply;
    }

    public static GetMovieHallByIdReply ToGetByIdReply(this CoreMovieHall hall)
    {
        GetMovieHallByIdReply reply = new()
        {
            Id = hall.Id.ToString(),
            Name = hall.Name,
            NumberOfSeats = hall.NumberOfSeats,
        };

        if (hall.MovieSessions is not null)
        {
            reply.MovieSessions.AddRange(hall.MovieSessions.Select(MovieSessionMappings.ToSession));
        }

        return reply;
    }

    public static CoreMovieHall ToMovieHall(this CreateMovieHallRequest request) =>
        new()
        {
            Name = request.Name,
            NumberOfSeats = request.NumberOfSeats
        };

    public static CreateMovieHallReply ToCreateReply(this CoreMovieHall hall)
    {
        CreateMovieHallReply reply = new()
        {
            Id = hall.Id.ToString(),
            Name = hall.Name,
            NumberOfSeats = hall.NumberOfSeats,
        };

        if (hall.MovieSessions is not null)
        {
            reply.MovieSessions.AddRange(hall.MovieSessions.Select(MovieSessionMappings.ToSession));
        }

        return reply;
    }

    public static void Update(this UpdateMovieHallRequest request, CoreMovieHall hall)
    {
        hall.Name = request.Name;
        hall.NumberOfSeats = request.NumberOfSeats;
    }
}