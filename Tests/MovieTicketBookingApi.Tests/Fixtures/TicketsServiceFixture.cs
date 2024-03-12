using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Bogus;
using Core.Interfaces.Helpers;
using Core.Interfaces.Repositories;
using Grpc.Core;
using Moq;
using MovieTicketBookingApi.AutoMapperProfiles;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.Movies;
using MovieTicketBookingApi.Protos.V1.Tickets;
using MovieTicketBookingApi.Services;
using MovieSession = Core.Entities.MovieSession;
using Ticket = Core.Entities.Ticket;

namespace MovieTicketBookingApi.Tests.Fixtures;

public class TicketsServiceFixture
{
	public TicketsServiceFixture()
	{
		var fixture = new Fixture().Customize(new AutoMoqCustomization());

		TicketId = Guid.NewGuid();
		MovieSessionId = Guid.NewGuid();

		var ticketFaker = new Faker<Ticket>()
			.RuleFor(t => t.Id, TicketId)
			.RuleFor(t => t.UserId, Guid.NewGuid())
			.RuleFor(t => t.MovieSessionId, MovieSessionId)
			.RuleFor(t => t.SeatNumber, f => f.Random.Int(1, 150))
			.RuleFor(t => t.IsCompleted, false);

		var movieSessionFaker = new Faker<MovieSession>()
			.RuleFor(ms => ms.Id, MovieSessionId)
			.RuleFor(ms => ms.MovieId, Guid.NewGuid())
			.RuleFor(ms => ms.MovieHallId, Guid.NewGuid())
			.RuleFor(ms => ms.DateTime, f => f.Date.Between(DateTime.UtcNow, DateTime.UtcNow.AddMonths(3)))
			.RuleFor(ms => ms.IsFinished, false)
			.RuleFor(ms => ms.Tickets, f => ticketFaker.Generate(f.Random.Int(10)));

		var getPaginatedDataRequestFaker = new Faker<GetPaginatedDataRequest>()
			.RuleFor(r => r.PageNumber, f => f.Random.Int(1, 25))
			.RuleFor(r => r.PageSize, f => f.Random.Int(1, 50));

		var getTicketByIdRequestFaker = new Faker<GetTicketByIdRequest>()
			.RuleFor(t => t.Id, TicketId.ToString());

		var createTicketRequestFaker = new Faker<CreateTicketRequest>()
			.RuleFor(t => t.UserId, Guid.NewGuid().ToString())
			.RuleFor(t => t.MovieSessionId, Guid.NewGuid().ToString())
			.RuleFor(t => t.SeatNumber, f => f.Random.Int(1, 150));

		var deleteTicketRequestFaker = new Faker<DeleteMovieRequest>()
			.RuleFor(t => t.Id, TicketId.ToString());

		TicketsRepository = fixture.Freeze<Mock<ITicketsRepository>>();
		MovieSessionsRepository = fixture.Freeze<Mock<IMovieSessionsRepository>>();
		CacheHelper = fixture.Freeze<Mock<ICacheHelper>>();
		ServerCallContext = fixture.Freeze<ServerCallContext>();

		Mapper = new MapperConfiguration(options => options.AddProfile<TicketsProfile>()).CreateMapper();

		TicketsService = new(
			TicketsRepository.Object,
			MovieSessionsRepository.Object,
			CacheHelper.Object,
			Mapper);

		Ticket = ticketFaker.Generate();
		Tickets = ticketFaker.Generate(Random.Shared.Next(2, 50));
		MovieSession = movieSessionFaker.Generate();
		GetTicketByIdRequest = getTicketByIdRequestFaker.Generate();
		GetPaginatedDataRequest = getPaginatedDataRequestFaker.Generate();
		CreateTicketRequest = createTicketRequestFaker.Generate();
	}

	public TicketsService TicketsService { get; }
	public Mock<ITicketsRepository> TicketsRepository { get; }
	public Mock<IMovieSessionsRepository> MovieSessionsRepository { get; }
	public Mock<ICacheHelper> CacheHelper { get; }
	public IMapper Mapper { get; }

	public Guid TicketId { get; }
	public Guid MovieSessionId { get; }
	public Ticket Ticket { get; }
	public IList<Ticket> Tickets { get; }
	public MovieSession MovieSession { get; }
	public ServerCallContext ServerCallContext { get; }
	public GetTicketByIdRequest GetTicketByIdRequest { get; }
	public GetPaginatedDataRequest GetPaginatedDataRequest { get; }
	public CreateTicketRequest CreateTicketRequest { get; }
}
