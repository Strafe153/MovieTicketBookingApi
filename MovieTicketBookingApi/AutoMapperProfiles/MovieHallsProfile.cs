using AutoMapper;
using MovieTicketBookingApi.Extensions;
using MovieTicketBookingApi.Protos.V1.MovieHalls;
using CoreMovieHall = Core.Entities.MovieHall;

namespace MovieTicketBookingApi.AutoMapperProfiles
{
    public class MovieHallsProfile : Profile
    {
        public MovieHallsProfile()
        {
            CreateMap<IList<CoreMovieHall>, GetAllMovieHallsReply>()
                .ForMember(r => r.MovieHalls, opt => opt.MapFrom(l => l.ToRepeatedField()));

            CreateMap<CoreMovieHall, GetMovieHallByIdReply>()
                .ForMember(r => r.Id, opt => opt.MapFrom(h => h.Id.ToString()))
                .ForMember(r => r.MovieSessions, opt => opt.MapFrom(h => h.MovieSessions.ToRepeatedField()));

            CreateMap<CreateMovieHallRequest, CoreMovieHall>();

            CreateMap<CoreMovieHall, CreateMovieHallReply>()
                .ForMember(r => r.Id, opt => opt.MapFrom(h => h.Id.ToString()))
                .ForMember(r => r.MovieSessions, opt => opt.MapFrom(h => h.MovieSessions.ToRepeatedField()));

            CreateMap<UpdateMovieHallRequest, CoreMovieHall>()
                .ForMember(h => h.Id, opt => opt.Ignore());
        }
    }
}
