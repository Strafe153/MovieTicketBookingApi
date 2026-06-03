using Google.Protobuf.WellKnownTypes;
using MovieTicketBookingApi.Protos.V1.Users;
using CoreUser = Domain.Entities.User;

namespace MovieTicketBookingApi.Mappings;

public static class UserMappings
{
    public static User ToUser(this CoreUser coreUser)
    {
        User user = new()
        {
            Id = coreUser.Id.ToString(),
            FirstName = coreUser.FirstName,
            LastName = coreUser.LastName,
            Email = coreUser.Email,
            BirthDate = Timestamp.FromDateTime(coreUser.BirthDate)
        };

        if (coreUser.Tickets is not null)
        {
            user.Tickets.AddRange(coreUser.Tickets.Select(TicketMappings.ToTicket));
        }

        return user;
    }

    public static GetAllUsersReply ToReply(this IList<CoreUser> list)
    {
        GetAllUsersReply reply = new();
        reply.Users.AddRange(list.Select(ToUser));

        return reply;
    }

    public static GetUserbyIdReply ToGetByIdReply(this CoreUser user)
    {
        GetUserbyIdReply reply = new()
        {
            Id = user.Id.ToString(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            BirthDate = Timestamp.FromDateTime(user.BirthDate)
        };

        if (user.Tickets is not null)
        {
            reply.Tickets.AddRange(user.Tickets.Select(TicketMappings.ToTicket));
        }

        return reply;
    }

    public static CoreUser ToUser(this RegisterUserRequest request) =>
        new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            BirthDate = request.BirthDate.ToDateTime()
        };

    public static RegisterUserReply ToRegisterReply(this CoreUser user) => new()
    {
        Id = user.Id.ToString(),
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        BirthDate = Timestamp.FromDateTime(user.BirthDate)
    };

	public static void Update(this UpdateUserRequest request, CoreUser user)
    {
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.BirthDate = request.BirthDate.ToDateTime();
    }
}