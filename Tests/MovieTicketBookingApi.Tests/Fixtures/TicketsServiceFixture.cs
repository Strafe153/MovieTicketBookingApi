using Domain.Interfaces.Helpers;
using Domain.Interfaces.Repositories;
using Grpc.Core;
using Moq;
using MovieTicketBookingApi.Services;

namespace MovieTicketBookingApi.Tests.Fixtures;

public class TicketsServiceFixture
{
	public Mock<ITicketsRepository> Repository { get; } = new();
	public Mock<ICacheHelper> CacheHelper { get; } = new();
	public Mock<ServerCallContext> ServerCallContext { get; } = new();

	public TicketsService CreateSut()
	{
		TicketsService service = new(
			Repository.Object,
			CacheHelper.Object);

		return service;
	}
}
