using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using MovieTicketBookingApi.Extensions;
using MovieTicketBookingApi.Protos.Tickets;
using CoreTicket = Core.Entities.Ticket;

namespace MovieTicketBookingApi.AutoMapperProfiles;

public class TicketsProfile : Profile
{
    public TicketsProfile()
    {
        CreateMap<IList<CoreTicket>, GetAllTicketsReply>()
            .ForMember(r => r.Tickets, opt => opt.MapFrom(l => l.ToRepeatedField()));

        CreateMap<CoreTicket, GetTicketByIdReply>()
            .ForMember(r => r.Id, opt => opt.MapFrom(t => t.Id.ToString()))
            .ForMember(r => r.DateTime, opt => opt.MapFrom(t => Timestamp.FromDateTime(t.DateTime)))
            .ForMember(r => r.MovieSessionId, opt => opt.MapFrom(t => t.MovieSessionId.ToString()))
            .ForMember(r => r.UserId, opt => opt.MapFrom(t => t.UserId.ToString()));

        CreateMap<CreateTicketRequest, CoreTicket>()
            .ForMember(t => t.DateTime, opt => opt.MapFrom(r => r.DateTime.ToDateTime()))
            .ForMember(t => t.MovieSessionId, opt => opt.MapFrom(r => new Guid(r.MovieSessionId)))
            .ForMember(t => t.UserId, opt => opt.MapFrom(r => new Guid(r.UserId)));

        CreateMap<CoreTicket, CreateTicketReply>()
            .ForMember(r => r.Id, opt => opt.MapFrom(t => t.Id.ToString()))
            .ForMember(r => r.DateTime, opt => opt.MapFrom(t => Timestamp.FromDateTime(t.DateTime)))
            .ForMember(r => r.MovieSessionId, opt => opt.MapFrom(t => t.MovieSessionId.ToString()))
            .ForMember(r => r.UserId, opt => opt.MapFrom(t => t.UserId.ToString()));
    }
}
