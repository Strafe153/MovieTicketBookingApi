using Domain.Shared.Constants;
using Moq;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.MovieHalls;
using MovieTicketBookingApi.Tests.Fixtures;
using Xunit;
using DateTime = System.DateTime;
using MovieHall = Domain.Entities.MovieHall;

namespace MovieTicketBookingApi.Tests;

public class MovieHallsServiceTests(MovieHallsServiceFixture fixture)
    : IClassFixture<MovieHallsServiceFixture>
{
    [Fact]
    public async Task GetAll_Should_ReturnGetAllMovieHallsReplyFromCache_WhenRequestIsValid()
    {
        // Arrange
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();

        List<MovieHall> halls = [
            new()
            {
                Id = firstId,
                Name = "The greate hall of cinema",
                NumberOfSeats = 120,
                MovieSessions = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                        MovieId = Guid.NewGuid(),
                        MovieHallId = firstId,
                        Tickets = []
                    }
                ]
            },
            new()
            {
                Id = secondId,
                Name = "Cinemator",
                NumberOfSeats = 80,
                MovieSessions = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        DateTime = new DateTime(2026, 10, 18, 16, 50, 0, DateTimeKind.Utc),
                        MovieId = Guid.NewGuid(),
                        MovieHallId = secondId,
                        Tickets = []
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
            $"{CacheConstants.MovieHallsPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";

        fixture.CacheHelper
            .Setup(h => h.Get<IList<MovieHall>>(cacheKey))
            .Returns(halls);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetAll(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.MovieHalls);
        Assert.True(result.MovieHalls.All(h => h.MovieSessions.Count > 0));
    }

    [Fact]
    public async Task GetAll_Should_ReturnGetAllMovieHallsReplyFromRepository_WhenRequestIsValid()
    {
        // Arrange
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();
        var pageNumber = 1;
        var pageSize = 5;

        List<MovieHall> halls = [
            new()
            {
                Id = firstId,
                Name = "The greate hall of cinema",
                NumberOfSeats = 120,
                MovieSessions = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                        MovieId = Guid.NewGuid(),
                        MovieHallId = firstId,
                        Tickets = []
                    }
                ]
            },
            new()
            {
                Id = secondId,
                Name = "Cinemator",
                NumberOfSeats = 80,
                MovieSessions = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        DateTime = new DateTime(2026, 10, 18, 16, 50, 0, DateTimeKind.Utc),
                        MovieId = Guid.NewGuid(),
                        MovieHallId = secondId,
                        Tickets = []
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
            $"{CacheConstants.MovieHallsPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";

        fixture.CacheHelper
            .Setup(h => h.Get<IList<MovieHall>>(cacheKey))
            .Returns((IList<MovieHall>)null!);

        fixture.Repository
            .Setup(r => r.GetAllAsync(pageNumber, pageSize))
            .ReturnsAsync(halls);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetAll(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.MovieHalls);
        Assert.True(result.MovieHalls.All(h => h.MovieSessions.Count > 0));
    }

    [Fact]
    public async Task GetById_Should_ReturnGetMovieHallByIdReplyFromCache_WhenMovieHallExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        MovieHall movieHall = new()
        {
            Id = id,
            Name = "The greate hall of cinema",
            NumberOfSeats = 120,
            MovieSessions = [
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                    MovieId = Guid.NewGuid(),
                    MovieHallId = id,
                    Tickets = []
                }
            ]
        };

        GetMovieHallByIdRequest request = new()
        {
            Id = idString
        };

        var cacheKey = $"{CacheConstants.MovieHallsPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<MovieHall>(cacheKey))
            .Returns(movieHall);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(idString, result.Id);
        Assert.Single(result.MovieSessions);
    }

    [Fact]
    public async Task GetById_Should_ReturnGetMovieHallByIdReplyFromRepository_WhenMovieHallExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        MovieHall movieHall = new()
        {
            Id = id,
            Name = "The greate hall of cinema",
            NumberOfSeats = 120,
            MovieSessions = [
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                    MovieId = Guid.NewGuid(),
                    MovieHallId = id,
                    Tickets = []
                }
            ]
        };

        GetMovieHallByIdRequest request = new()
        {
            Id = idString
        };

        var cacheKey = $"{CacheConstants.MovieHallsPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<MovieHall>(cacheKey))
            .Returns((MovieHall)null!);

        fixture.Repository
            .Setup(h => h.GetByIdAsync(idString))
            .ReturnsAsync(movieHall);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(idString, result.Id);
        Assert.Single(result.MovieSessions);
    }

    [Fact]
    public async Task GetById_Should_ThrowNullReferenceException_WhenMovieHallDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        MovieHall movieHall = new()
        {
            Id = id,
            Name = "The greate hall of cinema",
            NumberOfSeats = 120,
            MovieSessions = [
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                    MovieId = Guid.NewGuid(),
                    MovieHallId = id,
                    Tickets = []
                }
            ]
        };

        GetMovieHallByIdRequest request = new()
        {
            Id = Guid.NewGuid().ToString()
        };

        var cacheKey = $"{CacheConstants.MovieHallsPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<MovieHall>(cacheKey))
            .Returns((MovieHall)null!);

        fixture.Repository
            .Setup(h => h.GetByIdAsync(idString))
            .ReturnsAsync(movieHall);

        // Act
        var sut = fixture.CreateSut();
        Task<GetMovieHallByIdReply> result() => sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        await Assert.ThrowsAsync<NullReferenceException>(result);
    }

    [Fact]
    public async Task Create_Should_ReturnCreateMovieHallReply_WhenRequestIsValid()
    {
        // Arrange
        CreateMovieHallRequest request = new()
        {
            Name = "The greate hall of cinema",
            NumberOfSeats = 120
        };

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.Create(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotEmpty(result.Id);
        Assert.Equal("The greate hall of cinema", result.Name);
    }

    [Fact]
    public async Task Update_Should_ReturnEmptyReply_WhenMovieHallExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        MovieHall movieHall = new()
        {
            Id = id,
            Name = "The greate hall of cinema",
            NumberOfSeats = 120,
            MovieSessions = [
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                    MovieId = Guid.NewGuid(),
                    MovieHallId = id,
                    Tickets = []
                }
            ]
        };

        UpdateMovieHallRequest request = new()
        {
            Id = idString,
            Name = "Cinemator",
            NumberOfSeats = 80
        };

        fixture.Repository
            .Setup(r => r.GetByIdAsync(idString))
            .ReturnsAsync(movieHall);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.Update(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<EmptyReply>(result);
    }

    [Fact]
    public async Task Update_Should_ThrowNullReferenceException_WhenMovieHallDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();

        MovieHall movieHall = new()
        {
            Id = id,
            Name = "The greate hall of cinema",
            NumberOfSeats = 120,
            MovieSessions = [
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                    MovieId = Guid.NewGuid(),
                    MovieHallId = id,
                    Tickets = []
                }
            ]
        };

        UpdateMovieHallRequest request = new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Cinemator",
            NumberOfSeats = 80
        };

        fixture.Repository
            .Setup(r => r.GetByIdAsync(id.ToString()))
            .ReturnsAsync(movieHall);

        // Act
        var sut = fixture.CreateSut();
        Task<EmptyReply> result() => sut.Update(request, fixture.ServerCallContext.Object);

        // Assert
        await Assert.ThrowsAsync<NullReferenceException>(result);
    }

    [Fact]
    public async Task Delete_Should_ReturnEmptyReply_WhenMovieHallExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        MovieHall movieHall = new()
        {
            Id = id,
            Name = "The greate hall of cinema",
            NumberOfSeats = 120,
            MovieSessions = [
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                    MovieId = Guid.NewGuid(),
                    MovieHallId = id,
                    Tickets = []
                }
            ]
        };

        DeleteMovieHallRequest request = new()
        {
            Id = idString
        };

        fixture.Repository
            .Setup(r => r.GetByIdAsync(idString))
            .ReturnsAsync(movieHall);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.Delete(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<EmptyReply>(result);
    }

    [Fact]
    public async Task Delete_Should_ThrowNullReferenceException_WhenMovieHallDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();

        MovieHall movieHall = new()
        {
            Id = id,
            Name = "The greate hall of cinema",
            NumberOfSeats = 120,
            MovieSessions = [
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                    MovieId = Guid.NewGuid(),
                    MovieHallId = id,
                    Tickets = []
                }
            ]
        };

        DeleteMovieHallRequest request = new()
        {
            Id = Guid.NewGuid().ToString()
        };

        fixture.Repository
            .Setup(r => r.GetByIdAsync(id.ToString()))
            .ReturnsAsync(movieHall);

        // Act
        var sut = fixture.CreateSut();
        Task<EmptyReply> result() => sut.Delete(request, fixture.ServerCallContext.Object);

        // Assert
        await Assert.ThrowsAsync<NullReferenceException>(result);
    }
}
