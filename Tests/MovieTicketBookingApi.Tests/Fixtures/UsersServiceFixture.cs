using Domain.Interfaces.Helpers;
using Domain.Interfaces.Repositories;
using Grpc.Core;
using Moq;
using MovieTicketBookingApi.Services;

namespace MovieTicketBookingApi.Tests.Fixtures;

public class UsersServiceFixture
{
	public Mock<IUsersRepository> Repository { get; } = new();
	public Mock<IPasswordHelper> PasswordHelper { get; } = new();
	public Mock<ITokenHelper> TokenHelper { get; } = new();
	public Mock<ICacheHelper> CacheHelper { get; } = new();
	public Mock<IJobHelper> JobHelper { get; } = new();
	public Mock<ServerCallContext> ServerCallContext { get; } = new();

	public UsersService CreateSut()
	{
		UsersService service = new(
			Repository.Object,
			PasswordHelper.Object,
			TokenHelper.Object,
			CacheHelper.Object,
			JobHelper.Object);

		return service;
	}
}
