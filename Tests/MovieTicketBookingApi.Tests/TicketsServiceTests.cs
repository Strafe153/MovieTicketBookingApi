using FluentAssertions;
using Moq;
using MovieTicketBookingApi.Protos.V1.Tickets;
using MovieTicketBookingApi.Tests.Fixtures;
using Xunit;
using MovieSession = Domain.Entities.MovieSession;
using Ticket = Domain.Entities.Ticket;

namespace MovieTicketBookingApi.Tests;

public class TicketsServiceTests : IClassFixture<TicketsServiceFixture>
{
	private readonly TicketsServiceFixture _fixture;

	public TicketsServiceTests(TicketsServiceFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task GetAll_Should_ReturnGetAllTicketsReplyFromCache_WhenRequestIsValid()
	{
		// Arrange
		_fixture.CacheHelper
			.Setup(h => h.Get<IList<Ticket>>(It.IsAny<string>()))
			.Returns(_fixture.Tickets);

		// Act
		var result = await _fixture.TicketsService.GetAll(_fixture.GetPaginatedDataRequest, _fixture.ServerCallContext);

		// Assert
		result.Should().NotBeNull().And.BeOfType<GetAllTicketsReply>();
		result.Tickets.Should().NotBeEmpty();
	}

	[Fact]
	public async Task GetAll_Should_ReturnGetAllTicketsReplyFromRepository_WhenRequestIsValid()
	{
		// Arrange
		_fixture.CacheHelper
			.Setup(h => h.Get<IList<Ticket>>(It.IsAny<string>()))
			.Returns((IList<Ticket>)null!);

		_fixture.TicketsRepository
			.Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
			.ReturnsAsync(_fixture.Tickets);

		// Act
		var result = await _fixture.TicketsService.GetAll(_fixture.GetPaginatedDataRequest, _fixture.ServerCallContext);

		// Assert
		result.Should().NotBeNull().And.BeOfType<GetAllTicketsReply>();
		result.Tickets.Should().NotBeEmpty();
	}

	[Fact]
	public async Task GetById_Should_ReturnGetTicketByIdReplyFromCache_WhenTicketExists()
	{
		// Arrange
		_fixture.CacheHelper
			.Setup(h => h.Get<Ticket>(It.IsAny<string>()))
			.Returns(_fixture.Ticket);

		// Act
		var result = await _fixture.TicketsService.GetById(_fixture.GetTicketByIdRequest, _fixture.ServerCallContext);

		// Assert
		result.Should().NotBeNull().And.BeOfType<GetTicketByIdReply>();
	}

	[Fact]
	public async Task GetById_Should_ReturnGetTicketByIdReplyFromRepository_WhenTicketExists()
	{
		// Arrange
		_fixture.CacheHelper
			.Setup(h => h.Get<Ticket>(It.IsAny<string>()))
			.Returns((Ticket)null!);

		_fixture.TicketsRepository
			.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
			.ReturnsAsync(_fixture.Ticket);

		// Act
		var result = await _fixture.TicketsService.GetById(_fixture.GetTicketByIdRequest, _fixture.ServerCallContext);

		// Assert
		result.Should().NotBeNull().And.BeOfType<GetTicketByIdReply>();
	}

	[Fact]
	public async Task GetById_Should_ThrowNullReferenceException_WhenTicketDoesNotExist()
	{
		// Arrange
		_fixture.CacheHelper
			.Setup(h => h.Get<Ticket>(It.IsAny<string>()))
			.Returns((Ticket)null!);

		_fixture.TicketsRepository
			.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
			.ReturnsAsync((Ticket)null!);

		// Act
		var result = async () => await _fixture.TicketsService.GetById(_fixture.GetTicketByIdRequest, _fixture.ServerCallContext);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}

	[Fact]
	public async Task Create_Should_ReturnCreateTicketReply_WhenMovieSessionExists()
	{
		// Arrange
		_fixture.MovieSessionsRepository
			.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
			.ReturnsAsync(_fixture.MovieSession);

		// Act
		var result = await _fixture.TicketsService.Create(_fixture.CreateTicketRequest, _fixture.ServerCallContext);

		// Assert
		result.Should().NotBeNull().And.BeOfType<CreateTicketReply>();
	}

	[Fact]
	public async Task Create_Should_ThrowNullReferenceException_WhenMovieSessionDoesNotExist()
	{
		// Arrange
		_fixture.MovieSessionsRepository
			.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
			.ReturnsAsync((MovieSession)null!);

		// Act
		var result = async () => await _fixture.TicketsService.Create(_fixture.CreateTicketRequest, _fixture.ServerCallContext);

		// Assert
		await result.Should().ThrowAsync<NullReferenceException>();
	}
}
