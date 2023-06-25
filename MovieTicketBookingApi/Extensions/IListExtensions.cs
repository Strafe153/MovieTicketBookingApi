using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using CoreUser = Core.Entities.User;
using ProtoUser = MovieTicketBookingApi.Protos.V1.Users.User;
using CoreMovie = Core.Entities.Movie;
using CoreMovieHall = Core.Entities.MovieHall;
using CoreMovieSession = Core.Entities.MovieSession;
using CoreTicket = Core.Entities.Ticket;
using ProtoMovie = MovieTicketBookingApi.Protos.V1.Movies.Movie;
using ProtoMovieHall = MovieTicketBookingApi.Protos.V1.MovieHalls.MovieHall;
using ProtoMovieSession = MovieTicketBookingApi.Protos.V1.MovieSessions.MovieSession;
using ProtoTicket = MovieTicketBookingApi.Protos.V1.Tickets.Ticket;

namespace MovieTicketBookingApi.Extensions;

public static class IListExtensions
{
    public static RepeatedField<ProtoUser> ToRepeatedField(this IList<CoreUser> incoming)
    {
        var result = new RepeatedField<ProtoUser>();

        foreach (var item in incoming)
        {
            result.Add(new ProtoUser
            {
                Id = item.Id.ToString(),
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
                BirthDate = Timestamp.FromDateTime(item.BirthDate),
                Tickets = { item.Tickets.ToRepeatedField() }
            });
        }

        return result;
    }

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
                AgeRating = (Protos.V1.Movies.AgeRating)item.AgeRating,
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
