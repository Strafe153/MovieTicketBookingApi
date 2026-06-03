using Domain.Interfaces.Helpers;
using Domain.Interfaces.Repositories;
using Grpc.Core;
using Moq;
using MovieTicketBookingApi.Services;

namespace MovieTicketBookingApi.Tests.Fixtures;

public class MoviesServiceFixture
{
	public Mock<IMoviesRepository> Repository { get; } = new();
	public Mock<ICacheHelper> CacheHelper { get; } = new();
	public Mock<ServerCallContext> ServerCallContext { get; } = new();

	public MoviesService CreateSut() => new(Repository.Object, CacheHelper.Object);
}
