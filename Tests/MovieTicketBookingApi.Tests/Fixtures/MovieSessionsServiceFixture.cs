using Domain.Interfaces.Helpers;
using Domain.Interfaces.Repositories;
using Grpc.Core;
using Moq;
using MovieTicketBookingApi.Services;

namespace MovieTicketBookingApi.Tests.Fixtures;

public class MovieSessionsServiceFixture
{
	public Mock<IMovieSessionsRepository> Repository { get; } = new();
	public Mock<ICacheHelper> CacheHelper { get; } = new();
	public Mock<ServerCallContext> ServerCallContext { get; } = new();

	public MovieSessionsService CreateSut() => new(Repository.Object, CacheHelper.Object);
}
