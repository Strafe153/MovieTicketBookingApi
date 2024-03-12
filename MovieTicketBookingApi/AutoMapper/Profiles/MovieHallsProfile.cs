using AutoMapper;
using MovieTicketBookingApi.Protos.V1.MovieHalls;
using CoreMovieHall = Domain.Entities.MovieHall;

namespace MovieTicketBookingApi.AutoMapper.Profiles
{
	public class MovieHallsProfile : Profile
	{
		public MovieHallsProfile()
		{
			CreateMap<CoreMovieHall, MovieHall>();

			CreateMap<IList<CoreMovieHall>, GetAllMovieHallsReply>()
				.ForMember(dest => dest.MovieHalls, opt => opt.MapFrom(src => src));

			CreateMap<CoreMovieHall, GetMovieHallByIdReply>()
				.ForMember(dest => dest.MovieSessions, opt => opt.MapFrom(src => src.MovieSessions));

			CreateMap<CreateMovieHallRequest, CoreMovieHall>();

			CreateMap<CoreMovieHall, CreateMovieHallReply>()
				.ForMember(dest => dest.MovieSessions, opt => opt.MapFrom(src => src.MovieSessions));

			CreateMap<UpdateMovieHallRequest, CoreMovieHall>()
				.ForMember(dest => dest.Id, opt => opt.Ignore());
		}
	}
}
