using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using CoreMovie = Core.Entities.Movie;
using CoreMovieSession = Core.Entities.MovieSession;
using CoreMovieHall = Core.Entities.MovieHall;
using CoreTicket = Core.Entities.Ticket;
using ProtoMovie = MovieTicketBookingApi.Protos.Movies.Movie;
using ProtoMovieSession = MovieTicketBookingApi.Protos.MovieSessions.MovieSession;
using ProtoMovieHall = MovieTicketBookingApi.Protos.MovieHalls.MovieHall;
using ProtoTicket = MovieTicketBookingApi.Protos.Tickets.Ticket;

namespace MovieTicketBookingApi.Extensions;

public static class IListExtensions
{
    public static RepeatedField<ProtoMovie> ToRepeatedField(this IList<CoreMovie> incoming)
    {
        var result = new RepeatedField<ProtoMovie>();

        foreach (var item in incoming)
        {
            result.Add(new ProtoMovie
            {
                Id = item.Id.ToString(),
                Title = item.Title,
                DurationInMinutes = item.DurationInMinutes,
                AgeRating = (Protos.Movies.AgeRating)item.AgeRating,
                MovieSessions = { item.MovieSessions.ToRepeatedField() }
            });
        }

        return result;
    }

    public static RepeatedField<ProtoMovieSession> ToRepeatedField(this IList<CoreMovieSession> incoming)
    {
        var result = new RepeatedField<ProtoMovieSession>();

        foreach (var item in incoming)
        {
            result.Add(new ProtoMovieSession
            {
                Id = item.Id.ToString(),
                DateTime = Timestamp.FromDateTime(item.DateTime)
            });
        }

        return result;
    }

    public static RepeatedField<ProtoMovieHall> ToRepeatedField(this IList<CoreMovieHall> incoming)
    {
        var result = new RepeatedField<ProtoMovieHall>();

        foreach (var item in incoming)
        {
            result.Add(new ProtoMovieHall
            {
                Id = item.Id.ToString(),
                Name = item.Name,
                NumberOfSeats = item.NumberOfSeats,
                MovieSessions = { item.MovieSessions.ToRepeatedField() }
            });
        }

        return result;
    }

    public static RepeatedField<ProtoTicket> ToRepeatedField(this IList<CoreTicket> incoming)
    {
        var result = new RepeatedField<ProtoTicket>();

        foreach (var item in incoming)
        {
            result.Add(new ProtoTicket
            {
                Id = item.Id.ToString(),
                DateTime = Timestamp.FromDateTime(item.DateTime),
                SeatNumber = item.SeatNumber,
                MovieSessionId = item.MovieSessionId.ToString(),
                UserId = item.UserId.ToString()
            });
        }

        return result;
    }
}
