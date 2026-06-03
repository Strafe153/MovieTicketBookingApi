using Domain.Shared.Constants;
using Moq;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.Tickets;
using MovieTicketBookingApi.Tests.Fixtures;
using Xunit;
using Ticket = Domain.Entities.Ticket;

namespace MovieTicketBookingApi.Tests;

public class TicketsServiceTests(TicketsServiceFixture fixture) : IClassFixture<TicketsServiceFixture>
{
	[Fact]
	public async Task GetAll_Should_ReturnGetAllTicketsReplyFromCache_WhenRequestIsValid()
	{
		// Arrange
        List<Ticket> tickets = [
            new()
            {
                Id = Guid.NewGuid(),
                SeatNumber = 1,
                MovieSessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            },
            new()
            {
                Id = Guid.NewGuid(),
                SeatNumber = 6,
                MovieSessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            }
        ];

        GetPaginatedDataRequest request = new()
        {
            PageNumber = 1,
            PageSize = 5
        };

        var cacheKey =
            $"{CacheConstants.TicketsPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";

        fixture.CacheHelper
            .Setup(h => h.Get<IList<Ticket>>(cacheKey))
            .Returns(tickets);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetAll(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Tickets);
        Assert.Equal([1, 6], result.Tickets.Select(t => t.SeatNumber));
	}

	[Fact]
	public async Task GetAll_Should_ReturnGetAllTicketsReplyFromRepository_WhenRequestIsValid()
	{
		// Arrange
        var pageNumber = 1;
        var pageSize = 5;

        List<Ticket> tickets = [
            new()
            {
                Id = Guid.NewGuid(),
                SeatNumber = 1,
                MovieSessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            },
            new()
            {
                Id = Guid.NewGuid(),
                SeatNumber = 6,
                MovieSessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            }
        ];

        GetPaginatedDataRequest request = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var cacheKey =
            $"{CacheConstants.TicketsPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";

        fixture.CacheHelper
            .Setup(h => h.Get<IList<Ticket>>(cacheKey))
            .Returns(tickets);

        fixture.Repository
            .Setup(h => h.GetAllAsync(pageNumber, pageSize))
            .ReturnsAsync(tickets);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetAll(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Tickets);
        Assert.Equal([1, 6], result.Tickets.Select(t => t.SeatNumber));
	}

	[Fact]
	public async Task GetById_Should_ReturnGetTicketByIdReplyFromCache_WhenTicketExists()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        Ticket ticket = new()
        {
            Id = id,
            SeatNumber = 1,
            MovieSessionId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        GetTicketByIdRequest request = new()
        {
            Id = idString
        };

        var cacheKey = $"{CacheConstants.TicketsPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<Ticket>(cacheKey))
            .Returns(ticket);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(idString, result.Id);
        Assert.Equal(1, result.SeatNumber);
	}

	[Fact]
	public async Task GetById_Should_ReturnGetTicketByIdReplyFromRepository_WhenTicketExists()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        Ticket ticket = new()
        {
            Id = id,
            SeatNumber = 1,
            MovieSessionId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        GetTicketByIdRequest request = new()
        {
            Id = idString
        };

        var cacheKey = $"{CacheConstants.TicketsPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<Ticket>(cacheKey))
            .Returns((Ticket)null!);

        fixture.Repository
            .Setup(h => h.GetByIdAsync(idString))
            .ReturnsAsync(ticket);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(idString, result.Id);
        Assert.Equal(1, result.SeatNumber);
	}

	[Fact]
	public async Task GetById_Should_ThrowNullReferenceException_WhenTicketDoesNotExist()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        Ticket ticket = new()
        {
            Id = id,
            SeatNumber = 1,
            MovieSessionId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        GetTicketByIdRequest request = new()
        {
            Id = Guid.NewGuid().ToString()
        };

        var cacheKey = $"{CacheConstants.TicketsPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<Ticket>(cacheKey))
            .Returns((Ticket)null!);

        fixture.Repository
            .Setup(h => h.GetByIdAsync(idString))
            .ReturnsAsync(ticket);

        // Act
        var sut = fixture.CreateSut();
        Task<GetTicketByIdReply> result() => sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        await Assert.ThrowsAsync<NullReferenceException>(result);
    }

	[Fact]
	public async Task Create_Should_ReturnCreateTicketReply_WhenRequestIsValid()
	{
		// Arrange
        CreateTicketRequest request = new()
        {
            SeatNumber = 4,
            MovieSessionId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString()
        };

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.Create(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotEmpty(result.Id);
        Assert.Equal(4, result.SeatNumber);
	}
}
