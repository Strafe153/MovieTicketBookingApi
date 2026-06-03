using Domain.Shared.Constants;
using Google.Protobuf.WellKnownTypes;
using Moq;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.MovieSessions;
using MovieTicketBookingApi.Tests.Fixtures;
using Xunit;
using MovieSession = Domain.Entities.MovieSession;

namespace MovieTicketBookingApi.Tests;

public class MovieSessionsServiceTests(MovieSessionsServiceFixture fixture)
    : IClassFixture<MovieSessionsServiceFixture>
{
    [Fact]
    public async Task GetAll_Should_ReturnGetAllMovieSessionsReplyFromCache_WhenRequestIsValid()
    {
        // Arrange
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();

        List<MovieSession> sessions = [
            new()
            {
                Id = firstId,
                DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                MovieId = Guid.NewGuid(),
                MovieHallId = Guid.NewGuid(),
                Tickets = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        SeatNumber = 1,
                        MovieSessionId = firstId,
                        UserId = Guid.NewGuid()
                    }
                ]
            },
            new()
            {
                Id = secondId,
                DateTime = new DateTime(2026, 10, 18, 17, 30, 0, DateTimeKind.Utc),
                MovieId = Guid.NewGuid(),
                MovieHallId = Guid.NewGuid(),
                Tickets = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        SeatNumber = 33,
                        MovieSessionId = secondId,
                        UserId = Guid.NewGuid()
                    }
                ]
            }
        ];

        GetPaginatedDataRequest request = new()
        {
            PageNumber = 1,
            PageSize = 5
        };

        var cacheKey =
            $"{CacheConstants.MovieSessionsPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";

        fixture.CacheHelper
            .Setup(h => h.Get<IList<MovieSession>>(cacheKey))
            .Returns(sessions);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetAll(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.MovieSessions);
        Assert.True(result.MovieSessions.All(h => h.Tickets.Count > 0));
    }

    [Fact]
    public async Task GetAll_Should_ReturnGetAllMovieSessionsReplyFromRepository_WhenRequestIsValid()
    {
        // Arrange
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();
        var pageNumber = 1;
        var pageSize = 5;

        List<MovieSession> sessions = [
            new()
            {
                Id = firstId,
                DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                MovieId = Guid.NewGuid(),
                MovieHallId = Guid.NewGuid(),
                Tickets = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        SeatNumber = 1,
                        MovieSessionId = firstId,
                        UserId = Guid.NewGuid()
                    }
                ]
            },
            new()
            {
                Id = secondId,
                DateTime = new DateTime(2026, 10, 18, 17, 30, 0, DateTimeKind.Utc),
                MovieId = Guid.NewGuid(),
                MovieHallId = Guid.NewGuid(),
                Tickets = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        SeatNumber = 33,
                        MovieSessionId = secondId,
                        UserId = Guid.NewGuid()
                    }
                ]
            }
        ];

        GetPaginatedDataRequest request = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var cacheKey =
            $"{CacheConstants.MovieSessionsPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";

        fixture.CacheHelper
            .Setup(h => h.Get<IList<MovieSession>>(cacheKey))
            .Returns((IList<MovieSession>)null!);

        fixture.Repository
            .Setup(h => h.GetAllAsync(pageNumber, pageSize))
            .ReturnsAsync(sessions);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetAll(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.MovieSessions);
        Assert.True(result.MovieSessions.All(h => h.Tickets.Count > 0));
    }

    [Fact]
    public async Task GetById_Should_ReturnGetMovieSessionByIdReplyFromCache_WhenMovieSessionExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        MovieSession movieSession = new()
        {
            Id = id,
            DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
            MovieId = Guid.NewGuid(),
            MovieHallId = Guid.NewGuid(),
            Tickets = [
                new()
                {
                    Id = Guid.NewGuid(),
                    SeatNumber = 1,
                    MovieSessionId = id,
                    UserId = Guid.NewGuid()
                }
            ]
        };

        GetMovieSessionByIdRequest request = new()
        {
            Id = idString
        };

        var cacheKey = $"{CacheConstants.MovieSessionsPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<MovieSession>(cacheKey))
            .Returns(movieSession);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(idString, result.Id);
        Assert.Single(result.Tickets);
    }

    [Fact]
    public async Task GetById_Should_ReturnGetMovieSessionByIdReplyFromRepository_WhenMovieSessionExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        MovieSession movieSession = new()
        {
            Id = id,
            DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
            MovieId = Guid.NewGuid(),
            MovieHallId = Guid.NewGuid(),
            Tickets = [
                new()
                {
                    Id = Guid.NewGuid(),
                    SeatNumber = 1,
                    MovieSessionId = id,
                    UserId = Guid.NewGuid()
                }
            ]
        };

        GetMovieSessionByIdRequest request = new()
        {
            Id = idString
        };

        var cacheKey = $"{CacheConstants.MovieSessionsPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<MovieSession>(cacheKey))
            .Returns((MovieSession)null!);

        fixture.Repository
            .Setup(h => h.GetByIdAsync(idString))
            .ReturnsAsync(movieSession);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(idString, result.Id);
        Assert.Single(result.Tickets);
    }

    [Fact]
    public async Task GetById_Should_ThrowNullReferenceException_WhenMovieSessionDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        MovieSession movieSession = new()
        {
            Id = id,
            DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
            MovieId = Guid.NewGuid(),
            MovieHallId = Guid.NewGuid(),
            Tickets = [
                new()
                {
                    Id = Guid.NewGuid(),
                    SeatNumber = 1,
                    MovieSessionId = id,
                    UserId = Guid.NewGuid()
                }
            ]
        };

        GetMovieSessionByIdRequest request = new()
        {
            Id = Guid.NewGuid().ToString()
        };

        var cacheKey = $"{CacheConstants.MovieSessionsPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<MovieSession>(cacheKey))
            .Returns((MovieSession)null!);

        fixture.Repository
            .Setup(h => h.GetByIdAsync(idString))
            .ReturnsAsync(movieSession);

        // Act
        var sut = fixture.CreateSut();
        Task<GetMovieSessionByIdReply> result() => sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        await Assert.ThrowsAsync<NullReferenceException>(result);
    }

    [Fact]
    public async Task Create_Should_ReturnCreateMovieSessionReply_WhenRequestIsValid()
    {
        // Arrange
        DateTime dateTime = new(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc);

        CreateMovieSessionRequest request = new()
        {
            DateTime = Timestamp.FromDateTime(dateTime),
            MovieId = Guid.NewGuid().ToString(),
            MovieHallId = Guid.NewGuid().ToString()
        };

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.Create(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotEmpty(result.Id);
        Assert.Equal(dateTime, result.DateTime.ToDateTime());
    }

    [Fact]
    public async Task Update_Should_ReturnEmptyReply_WhenMovieSessionExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        MovieSession movieSession = new()
        {
            Id = id,
            DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
            MovieId = Guid.NewGuid(),
            MovieHallId = Guid.NewGuid(),
            Tickets = [
                new()
                {
                    Id = Guid.NewGuid(),
                    SeatNumber = 1,
                    MovieSessionId = id,
                    UserId = Guid.NewGuid()
                }
            ]
        };

        UpdateMovieSessionRequest request = new()
        {
            Id = idString,
            DateTime = Timestamp.FromDateTime(
                new DateTime(2026, 10, 20, 10, 50, 0, DateTimeKind.Utc)),
            MovieId = Guid.NewGuid().ToString(),
            MovieHallId = Guid.NewGuid().ToString()
        };

        var cacheKey = $"{CacheConstants.MovieSessionsPrefix}:{request.Id}";

        fixture.Repository
            .Setup(h => h.GetByIdAsync(idString))
            .ReturnsAsync(movieSession);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.Update(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<EmptyReply>(result);
    }

    [Fact]
    public async Task Update_Should_ThrowNullReferenceException_WhenMovieSessionDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();

        MovieSession movieSession = new()
        {
            Id = id,
            DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
            MovieId = Guid.NewGuid(),
            MovieHallId = Guid.NewGuid(),
            Tickets = [
                new()
                {
                    Id = Guid.NewGuid(),
                    SeatNumber = 1,
                    MovieSessionId = id,
                    UserId = Guid.NewGuid()
                }
            ]
        };

        UpdateMovieSessionRequest request = new()
        {
            Id = Guid.NewGuid().ToString(),
            DateTime = Timestamp.FromDateTime(
                new DateTime(2026, 10, 20, 10, 50, 0, DateTimeKind.Utc)),
            MovieId = Guid.NewGuid().ToString(),
            MovieHallId = Guid.NewGuid().ToString()
        };

        var cacheKey = $"{CacheConstants.MovieSessionsPrefix}:{request.Id}";

        fixture.Repository
            .Setup(h => h.GetByIdAsync(id.ToString()))
            .ReturnsAsync(movieSession);

        // Act
        var sut = fixture.CreateSut();
        Task<EmptyReply> result() => sut.Update(request, fixture.ServerCallContext.Object);

        // Assert
        await Assert.ThrowsAsync<NullReferenceException>(result);
    }
}
