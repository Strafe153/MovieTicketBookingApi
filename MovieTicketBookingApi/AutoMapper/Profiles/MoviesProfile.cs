using AutoMapper;
using MovieTicketBookingApi.Protos.V1.Movies;
using CoreMovie = Domain.Entities.Movie;
using CoreAgeRating = Domain.Enums.AgeRating;

namespace MovieTicketBookingApi.AutoMapperProfiles;

public class MoviesProfile : Profile
{
	public MoviesProfile()
	{
		CreateMap<CoreMovie, Movie>();

		CreateMap<IList<CoreMovie>, GetAllMoviesReply>()
			.ForMember(dest => dest.Movies, opt => opt.MapFrom(src => src));

		CreateMap<CoreMovie, GetMovieByIdReply>();

		CreateMap<CreateMovieRequest, CoreMovie>()
			.ForMember(dest => dest.AgeRating, opt => opt.MapFrom(src => (CoreAgeRating)src.AgeRating));

		CreateMap<CoreMovie, CreateMovieReply>()
			.ForMember(dest => dest.AgeRating, opt => opt.MapFrom(src => (AgeRating)src.AgeRating));

		CreateMap<UpdateMovieRequest, CoreMovie>()
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.AgeRating, opt => opt.MapFrom(src => (CoreAgeRating)src.AgeRating));
	}
}
