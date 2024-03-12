using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Bogus;
using Core.Interfaces.Helpers;
using Core.Interfaces.Repositories;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Moq;
using MovieTicketBookingApi.AutoMapper.Profiles;
using MovieTicketBookingApi.AutoMapperProfiles;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.MovieSessions;
using MovieTicketBookingApi.Services;
using MovieSession = Core.Entities.MovieSession;
using Ticket = Core.Entities.Ticket;

namespace MovieTicketBookingApi.Tests.Fixtures;

public class MovieSessionsServiceFixture
{
	public MovieSessionsServiceFixture()
	{
		var fixture = new Fixture().Customize(new AutoMoqCustomization());

		Id = Guid.NewGuid();

		var ticketFaker = new Faker<Ticket>()
			.RuleFor(t => t.Id, Guid.NewGuid())
			.RuleFor(t => t.SeatNumber, f => f.Random.Int())
			.RuleFor(t => t.UserId, Guid.NewGuid())
			.RuleFor(t => t.IsCompleted, false);

		var movieSessionFaker = new Faker<MovieSession>()
			.RuleFor(ms => ms.Id, Id)
			.RuleFor(ms => ms.MovieId, Guid.NewGuid())
			.RuleFor(ms => ms.MovieHallId, Guid.NewGuid())
			.RuleFor(ms => ms.DateTime, f => f.Date.Between(DateTime.UtcNow, DateTime.UtcNow.AddMonths(3)))
			.RuleFor(ms => ms.IsFinished, false)
			.RuleFor(ms => ms.Tickets, f => ticketFaker.Generate(f.Random.Int(10)));

		var getPaginatedDataRequestFaker = new Faker<GetPaginatedDataRequest>()
			.RuleFor(r => r.PageNumber, f => f.Random.Int(1, 25))
			.RuleFor(r => r.PageSize, f => f.Random.Int(1, 50));

		var getMovieSessionByIdRequestFaker = new Faker<GetMovieSessionByIdRequest>()
			.RuleFor(r => r.Id, Id.ToString());

		var createMovieHallRequestFaker = new Faker<CreateMovieSessionRequest>()
			.RuleFor(r => r.MovieId, Guid.NewGuid().ToString())
			.RuleFor(r => r.MovieHallId, Guid.NewGuid().ToString())
			.RuleFor(r => r.DateTime, f => f.Date.Between(DateTime.UtcNow, DateTime.UtcNow.AddMonths(3)).ToTimestamp());

		var updateMovieSessionRequestFaker = new Faker<UpdateMovieSessionRequest>()
			.RuleFor(r => r.Id, Id.ToString())
			.RuleFor(r => r.MovieId, Guid.NewGuid().ToString())
			.RuleFor(r => r.MovieHallId, Guid.NewGuid().ToString())
			.RuleFor(r => r.DateTime, f => f.Date.Between(DateTime.UtcNow, DateTime.UtcNow.AddMonths(3)).ToTimestamp());

		MovieSessionsRepository = fixture.Freeze<Mock<IMovieSessionsRepository>>();
		CacheHelper = fixture.Freeze<Mock<ICacheHelper>>();
		ServerCallContext = fixture.Freeze<ServerCallContext>();

		Mapper = new MapperConfiguration(options =>
		{
			options.AddProfile<TimestampDateTimeProfile>();
			options.AddProfile<TicketsProfile>();
			options.AddProfile<MovieSessionsProfile>();
		}).CreateMapper();

		MovieSessionsService = new(
			MovieSessionsRepository.Object,
			CacheHelper.Object,
			Mapper);

		MovieSession = movieSessionFaker.Generate();
		MovieSessions = movieSessionFaker.Generate(Random.Shared.Next(2, 50));
		GetMovieSessionByIdRequest = getMovieSessionByIdRequestFaker.Generate();
		GetPaginatedDataRequest = getPaginatedDataRequestFaker.Generate();
		CreateMovieSessionRequest = createMovieHallRequestFaker.Generate();
		UpdateMovieSessionRequest = updateMovieSessionRequestFaker.Generate();
	}

	public MovieSessionsService MovieSessionsService { get; }
	public Mock<IMovieSessionsRepository> MovieSessionsRepository { get; }
	public Mock<ICacheHelper> CacheHelper { get; }
	public IMapper Mapper { get; }

	public Guid Id { get; }
	public MovieSession MovieSession { get; }
	public IList<MovieSession> MovieSessions { get; }
	public ServerCallContext ServerCallContext { get; }
	public GetMovieSessionByIdRequest GetMovieSessionByIdRequest { get; }
	public GetPaginatedDataRequest GetPaginatedDataRequest { get; }
	public CreateMovieSessionRequest CreateMovieSessionRequest { get; }
	public UpdateMovieSessionRequest UpdateMovieSessionRequest { get; }
}
