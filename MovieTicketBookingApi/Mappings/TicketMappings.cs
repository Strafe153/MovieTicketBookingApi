using MovieTicketBookingApi.Protos.V1.Tickets;
using CoreTicket = Domain.Entities.Ticket;

namespace MovieTicketBookingApi.Mappings;

public static class TicketMappings
{
    public static Ticket ToTicket(this CoreTicket ticket) => new()
    {
        Id = ticket.Id.ToString(),
        SeatNumber = ticket.SeatNumber,
        MovieSessionId = ticket.MovieSessionId.ToString(),
        UserId = ticket.UserId.ToString()
    };

    public static GetAllTicketsReply ToReply(this IList<CoreTicket> list)
    {
        GetAllTicketsReply reply = new();
        reply.Tickets.AddRange(list.Select(ToTicket));

        return reply;
    }

    public static GetTicketByIdReply ToGetByIdReply(this CoreTicket ticket) => new()
    {
        Id = ticket.Id.ToString(),
        SeatNumber = ticket.SeatNumber,
        MovieSessionId = ticket.MovieSessionId.ToString(),
        UserId = ticket.UserId.ToString()
    };

    public static CoreTicket ToTicket(this CreateTicketRequest request) =>
        new()
        {
            SeatNumber = request.SeatNumber,
            MovieSessionId = Guid.Parse(request.MovieSessionId),
            UserId = Guid.Parse(request.UserId)
        };

    public static CreateTicketReply ToCreateReply(this CoreTicket ticket) => new()
    {
        Id = ticket.Id.ToString(),
        SeatNumber = ticket.SeatNumber,
        MovieSessionId = ticket.MovieSessionId.ToString(),
        UserId = ticket.UserId.ToString()
    };
}