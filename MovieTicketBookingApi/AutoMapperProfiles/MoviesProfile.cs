using AutoMapper;
using MovieTicketBookingApi.Extensions;
using MovieTicketBookingApi.Protos.Movies;
using CoreMovie = Core.Entities.Movie;

namespace MovieTicketBookingApi.AutoMapperProfiles;

public class MoviesProfile : Profile
{
    public MoviesProfile()
    {
        CreateMap<IList<CoreMovie>, GetAllMoviesReply>()
            .ForMember(r => r.Movies, opt => opt.MapFrom(l => l.ToRepeatedField()));

        CreateMap<CoreMovie, GetMovieByIdReply>()
            .ForMember(r => r.Id, opt => opt.MapFrom(m => m.Id.ToString()))
            .ForMember(r => r.MovieSessions, opt => opt.MapFrom(m => m.MovieSessions.ToRepeatedField()));

        CreateMap<CreateMovieRequest, CoreMovie>()
            .ForMember(m => m.AgeRating, opt => opt.MapFrom(r => (Core.Enums.AgeRating)r.AgeRating));

        CreateMap<CoreMovie, CreateMovieReply>()
            .ForMember(r => r.Id, opt => opt.MapFrom(m => m.Id.ToString()))
            .ForMember(r => r.AgeRating, opt => opt.MapFrom(r => (AgeRating)r.AgeRating));

        CreateMap<UpdateMovieRequest, CoreMovie>()
            .ForMember(m => m.Id, opt => opt.Ignore())
            .ForMember(m => m.AgeRating, opt => opt.MapFrom(r => (Core.Enums.AgeRating)r.AgeRating));
    }
}
