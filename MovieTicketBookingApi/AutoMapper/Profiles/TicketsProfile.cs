using AutoMapper;
using MovieTicketBookingApi.Protos.V1.Tickets;
using CoreTicket = Domain.Entities.Ticket;

namespace MovieTicketBookingApi.AutoMapperProfiles;

public class TicketsProfile : Profile
{
	public TicketsProfile()
	{
		CreateMap<CoreTicket, Ticket>();

		CreateMap<IList<CoreTicket>, GetAllTicketsReply>()
			.ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src));

		CreateMap<CoreTicket, GetTicketByIdReply>();

		CreateMap<CreateTicketRequest, CoreTicket>();

		CreateMap<CoreTicket, CreateTicketReply>();
	}
}
