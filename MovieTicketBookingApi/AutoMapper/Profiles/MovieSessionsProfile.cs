using AutoMapper;
using MovieTicketBookingApi.Protos.V1.MovieSessions;
using CoreMovieSession = Domain.Entities.MovieSession;

namespace MovieTicketBookingApi.AutoMapperProfiles;

public class MovieSessionsProfile : Profile
{
	public MovieSessionsProfile()
	{
		CreateMap<CoreMovieSession, MovieSession>();

		CreateMap<IList<CoreMovieSession>, GetAllMovieSessionsReply>()
			.ForMember(dest => dest.MovieSessions, opt => opt.MapFrom(src => src));

		CreateMap<CoreMovieSession, GetMovieSessionByIdReply>()
			.ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets));

		CreateMap<CreateMovieSessionRequest, CoreMovieSession>();

		CreateMap<CoreMovieSession, CreateMovieSessionReply>();

		CreateMap<UpdateMovieSessionRequest, CoreMovieSession>()
			.ForMember(dest => dest.Id, opt => opt.Ignore());
	}
}
