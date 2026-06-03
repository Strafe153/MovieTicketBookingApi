using Google.Protobuf.WellKnownTypes;
using MovieTicketBookingApi.Protos.V1.MovieSessions;
using CoreMovieSession = Domain.Entities.MovieSession;

namespace MovieTicketBookingApi.Mappings;

public static class MovieSessionMappings
{
    public static MovieSession ToSession(this CoreMovieSession coreSession)
    {
        MovieSession session = new()
        {
            Id = coreSession.Id.ToString(),
            DateTime = Timestamp.FromDateTime(coreSession.DateTime),
            MovieId = coreSession.MovieId.ToString(),
            MovieHallId = coreSession.MovieHallId.ToString()
        };

        if (coreSession.Tickets is not null)
        {
            session.Tickets.AddRange(coreSession.Tickets.Select(TicketMappings.ToTicket));
        }

        return session;
    }

    public static GetAllMovieSessionsReply ToReply(this IList<CoreMovieSession> list)
    {
        GetAllMovieSessionsReply reply = new();
        reply.MovieSessions.AddRange(list.Select(ToSession));

        return reply;
    }

    public static GetMovieSessionByIdReply ToGetByIdReply(this CoreMovieSession coreSession)
    {
        GetMovieSessionByIdReply reply = new()
        {
            Id = coreSession.Id.ToString(),
            DateTime = Timestamp.FromDateTime(coreSession.DateTime),
            MovieId = coreSession.MovieId.ToString(),
            MovieHallId = coreSession.MovieHallId.ToString()
        };

        if (coreSession.Tickets is not null)
        {
            reply.Tickets.AddRange(coreSession.Tickets.Select(TicketMappings.ToTicket));
        }

        return reply;
    }

    public static CoreMovieSession ToSession(this CreateMovieSessionRequest request) =>
        new()
        {
            DateTime = request.DateTime.ToDateTime(),
            MovieId = Guid.Parse(request.MovieId),
            MovieHallId = Guid.Parse(request.MovieHallId)
        };

    public static CreateMovieSessionReply ToCreateReply(this CoreMovieSession session) => new()
    {
        Id = session.Id.ToString(),
        DateTime = Timestamp.FromDateTime(session.DateTime),
        MovieId = session.MovieId.ToString(),
        MovieHallId = session.MovieHallId.ToString()
    };

    public static void Update(this UpdateMovieSessionRequest request, CoreMovieSession session)
    {
        session.DateTime = request.DateTime.ToDateTime();
        session.MovieId = Guid.Parse(request.MovieId);
        session.MovieHallId = Guid.Parse(request.MovieHallId);
    }
}