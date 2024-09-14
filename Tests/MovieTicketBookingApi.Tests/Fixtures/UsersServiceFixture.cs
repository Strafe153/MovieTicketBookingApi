using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Bogus;
using Domain.Interfaces.Helpers;
using Domain.Interfaces.Repositories;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Moq;
using MovieTicketBookingApi.AutoMapperProfiles;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.Users;
using MovieTicketBookingApi.Services;
using Ticket = Domain.Entities.Ticket;
using User = Domain.Entities.User;

namespace MovieTicketBookingApi.Tests.Fixtures;

public class UsersServiceFixture
{
	public UsersServiceFixture()
	{
		var fixture = new Fixture().Customize(new AutoMoqCustomization());

		Id = Guid.NewGuid();

		var ticketFaker = new Faker<Ticket>()
			.RuleFor(t => t.Id, Guid.NewGuid())
			.RuleFor(t => t.UserId, Id)
			.RuleFor(t => t.MovieSessionId, Guid.NewGuid())
			.RuleFor(t => t.SeatNumber, f => f.Random.Int(1, 150))
			.RuleFor(t => t.IsCompleted, false);

		var userFaker = new Faker<User>()
			.RuleFor(u => u.Id, Id)
			.RuleFor(u => u.FirstName, f => f.Name.FirstName())
			.RuleFor(u => u.LastName, f => f.Name.LastName())
			.RuleFor(u => u.Email, f => f.Internet.Email())
			.RuleFor(u => u.BirthDate, f => f.Date.Past())
			.RuleFor(u => u.PasswordHash, f => f.Random.Bytes(64))
			.RuleFor(u => u.PasswordSalt, f => f.Random.Bytes(32))
			.RuleFor(t => t.Tickets, f => ticketFaker.Generate(f.Random.Int(2, 50)))
			.RuleFor(u => u.IsActive, true);

		var getPaginatedDataRequestFaker = new Faker<GetPaginatedDataRequest>()
			.RuleFor(r => r.PageNumber, f => f.Random.Int(1, 25))
			.RuleFor(r => r.PageSize, f => f.Random.Int(1, 50));

		var getUserByIdRequestFaker = new Faker<GetUserByIdRequest>()
			.RuleFor(u => u.Id, Id.ToString());

		var registerUserRequestFaker = new Faker<RegisterUserRequest>()
			.RuleFor(u => u.FirstName, f => f.Name.FirstName())
			.RuleFor(u => u.LastName, f => f.Name.LastName())
			.RuleFor(u => u.Email, f => f.Internet.Email())
			.RuleFor(u => u.BirthDate, f => f.Date.Past().ToTimestamp())
			.RuleFor(u => u.Password, f => f.Internet.Password());

		var loginUserRequestFaker = new Faker<LoginUserRequest>()
			.RuleFor(u => u.Email, f => f.Internet.Email())
			.RuleFor(u => u.Password, f => f.Internet.Password());

		UsersRepository = fixture.Freeze<Mock<IUsersRepository>>();
		PasswordHelper = fixture.Freeze<Mock<IPasswordHelper>>();
		TokenHelper = fixture.Freeze<Mock<ITokenHelper>>();
		CacheHelper = fixture.Freeze<Mock<ICacheHelper>>();
		JobHelper = fixture.Freeze<Mock<IJobHelper>>();
		ServerCallContext = fixture.Freeze<ServerCallContext>();

		Mapper = new MapperConfiguration(options => options.AddProfile<UserProfile>()).CreateMapper();

		UsersService = new(
			UsersRepository.Object,
			PasswordHelper.Object,
			TokenHelper.Object,
			CacheHelper.Object,
			JobHelper.Object,
			Mapper);

		User = userFaker.Generate();
		Users = userFaker.Generate(Random.Shared.Next(2, 50));
		GetUserByIdRequest = getUserByIdRequestFaker.Generate();
		GetPaginatedDataRequest = getPaginatedDataRequestFaker.Generate();
		RegisterUserRequest = registerUserRequestFaker.Generate();
		LoginUserRequest = loginUserRequestFaker.Generate();
	}

	public UsersService UsersService { get; }
	public Mock<IUsersRepository> UsersRepository { get; }
	public Mock<IPasswordHelper> PasswordHelper { get; }
	public Mock<ITokenHelper> TokenHelper { get; }
	public Mock<ICacheHelper> CacheHelper { get; }
	public Mock<IJobHelper> JobHelper { get; set; }
	public IMapper Mapper { get; }

	public Guid Id { get; }
	public User User { get; }
	public IList<User> Users { get; }
	public ServerCallContext ServerCallContext { get; }
	public GetUserByIdRequest GetUserByIdRequest { get; }
	public GetPaginatedDataRequest GetPaginatedDataRequest { get; }
	public RegisterUserRequest RegisterUserRequest { get; }
	public LoginUserRequest LoginUserRequest { get; }
}
