using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Bogus;
using Core.Interfaces.Helpers;
using Core.Interfaces.Repositories;
using Core.Shared;
using Grpc.Core;
using Moq;
using MovieTicketBookingApi.AutoMapperProfiles;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.Movies;
using MovieTicketBookingApi.Services;
using CoreAgeRating = Core.Enums.AgeRating;
using Movie = Core.Entities.Movie;

namespace MovieTicketBookingApi.Tests.Fixtures;

public class MoviesServiceFixture
{
	public MoviesServiceFixture()
	{
		var fixture = new Fixture().Customize(new AutoMoqCustomization());

		Id = Guid.NewGuid();

		var movieHallFaker = new Faker<Movie>()
			.RuleFor(m => m.Id, Id)
			.RuleFor(m => m.Title, f => f.Random.Word())
			.RuleFor(m => m.AgeRating, f => (CoreAgeRating)f.Random.Int(Enum.GetValues(typeof(CoreAgeRating)).Length))
			.RuleFor(m => m.DurationInMinutes, f => f.Random.Int(30, 300));

		var getPaginatedDataRequestFaker = new Faker<GetPaginatedDataRequest>()
			.RuleFor(r => r.PageNumber, f => f.Random.Int(1, 25))
			.RuleFor(r => r.PageSize, f => f.Random.Int(1, 50));

		var getMovieByIdRequestFaker = new Faker<GetMovieByIdRequest>()
			.RuleFor(r => r.Id, Id.ToString());

		var createMovieRequestFaker = new Faker<CreateMovieRequest>()
			.RuleFor(r => r.Title, f => f.Random.Word())
			.RuleFor(r => r.AgeRating, f => (AgeRating)f.Random.Int(Enum.GetValues(typeof(CoreAgeRating)).Length))
			.RuleFor(r => r.DurationInMinutes, f => f.Random.Int(30, 300));

		var updateMovieRequestFaker = new Faker<UpdateMovieRequest>()
			.RuleFor(r => r.Id, Id.ToString())
			.RuleFor(r => r.Title, f => f.Random.Word())
			.RuleFor(r => r.AgeRating, f => (AgeRating)f.Random.Int(Enum.GetValues(typeof(AgeRating)).Length))
			.RuleFor(r => r.DurationInMinutes, f => f.Random.Int(30, 300));

		var deleteMovieRequestFaker = new Faker<DeleteMovieRequest>()
			.RuleFor(r => r.Id, Id.ToString());

		MoviesRepository = fixture.Freeze<Mock<IMoviesRepository>>();
		CacheHelper = fixture.Freeze<Mock<ICacheHelper>>();
		ServerCallContext = fixture.Freeze<ServerCallContext>();

		Mapper = new MapperConfiguration(options => options.AddProfile<MoviesProfile>()).CreateMapper();

		MoviesService = new(
			MoviesRepository.Object,
			CacheHelper.Object,
			Mapper);

		Movie = movieHallFaker.Generate();
		Movies = movieHallFaker.Generate(Random.Shared.Next(2, 50));
		GetMovieByIdRequest = getMovieByIdRequestFaker.Generate();
		GetPaginatedDataRequest = getPaginatedDataRequestFaker.Generate();
		CreateMovieRequest = createMovieRequestFaker.Generate();
		UpdateMovieRequest = updateMovieRequestFaker.Generate();
		DeleteMovieRequest = deleteMovieRequestFaker.Generate();
	}

	public MoviesService MoviesService { get; }
	public Mock<IMoviesRepository> MoviesRepository { get; }
	public Mock<ICacheHelper> CacheHelper { get; }
	public IMapper Mapper { get; }

	public Guid Id { get; }
	public Movie Movie { get; }
	public IList<Movie> Movies { get; }
	public ServerCallContext ServerCallContext { get; }
	public GetMovieByIdRequest GetMovieByIdRequest { get; }
	public GetPaginatedDataRequest GetPaginatedDataRequest { get; }
	public CreateMovieRequest CreateMovieRequest { get; }
	public UpdateMovieRequest UpdateMovieRequest { get; }
	public DeleteMovieRequest DeleteMovieRequest { get; }
}
