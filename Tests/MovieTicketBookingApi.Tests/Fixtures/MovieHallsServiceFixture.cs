using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Bogus;
using Domain.Interfaces.Helpers;
using Domain.Interfaces.Repositories;
using Grpc.Core;
using Moq;
using MovieTicketBookingApi.AutoMapper.Profiles;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.MovieHalls;
using MovieTicketBookingApi.Services;
using MovieHall = Domain.Entities.MovieHall;

namespace MovieTicketBookingApi.Tests.Fixtures;

public class MovieHallsServiceFixture
{
	public MovieHallsServiceFixture()
	{
		var fixture = new Fixture().Customize(new AutoMoqCustomization());

		Id = Guid.NewGuid();

		var movieHallFaker = new Faker<MovieHall>()
			.RuleFor(mh => mh.Id, Id)
			.RuleFor(mh => mh.Name, f => f.Name.JobArea())
			.RuleFor(mh => mh.NumberOfSeats, f => f.Random.Int())
			.RuleFor(mh => mh.IsActive, true);

		var getPaginatedDataRequestFaker = new Faker<GetPaginatedDataRequest>()
			.RuleFor(r => r.PageNumber, f => f.Random.Int(1, 25))
			.RuleFor(r => r.PageSize, f => f.Random.Int(1, 50));

		var getMovieHallByIdRequestFaker = new Faker<GetMovieHallByIdRequest>()
			.RuleFor(r => r.Id, Id.ToString());

		var createMovieHallRequestFaker = new Faker<CreateMovieHallRequest>()
			.RuleFor(r => r.Name, f => f.Name.JobArea())
			.RuleFor(r => r.NumberOfSeats, f => f.Random.Int());

		var updateMovieHallRequestFaker = new Faker<UpdateMovieHallRequest>()
			.RuleFor(r => r.Id, Id.ToString())
			.RuleFor(r => r.Name, f => f.Name.JobArea())
			.RuleFor(r => r.NumberOfSeats, f => f.Random.Int());

		var deleteMovieHallRequestFaker = new Faker<DeleteMovieHallRequest>()
			.RuleFor(r => r.Id, Id.ToString());

		MovieHallsRepository = fixture.Freeze<Mock<IMovieHallsRepository>>();
		CacheHelper = fixture.Freeze<Mock<ICacheHelper>>();
		ServerCallContext = fixture.Freeze<ServerCallContext>();

		Mapper = new MapperConfiguration(options => options.AddProfile<MovieHallsProfile>()).CreateMapper();

		MovieHallsService = new(
			MovieHallsRepository.Object,
			CacheHelper.Object,
			Mapper);

		MovieHall = movieHallFaker.Generate();
		MovieHalls = movieHallFaker.Generate(Random.Shared.Next(2, 50));
		GetMovieHallByIdRequest = getMovieHallByIdRequestFaker.Generate();
		GetPaginatedDataRequest = getPaginatedDataRequestFaker.Generate();
		CreateMovieHallRequest = createMovieHallRequestFaker.Generate();
		UpdateMovieHallRequest = updateMovieHallRequestFaker.Generate();
		DeleteMovieHallRequest = deleteMovieHallRequestFaker.Generate();
	}

	public MovieHallsService MovieHallsService { get; }
	public Mock<IMovieHallsRepository> MovieHallsRepository { get; }
	public Mock<ICacheHelper> CacheHelper { get; }
	public IMapper Mapper { get; }

	public Guid Id { get; }
	public MovieHall MovieHall { get; }
	public IList<MovieHall> MovieHalls { get; }
	public ServerCallContext ServerCallContext { get; }
	public GetMovieHallByIdRequest GetMovieHallByIdRequest { get; }
	public GetPaginatedDataRequest GetPaginatedDataRequest { get; }
	public CreateMovieHallRequest CreateMovieHallRequest { get; }
	public UpdateMovieHallRequest UpdateMovieHallRequest { get; }
	public DeleteMovieHallRequest DeleteMovieHallRequest { get; }
}
