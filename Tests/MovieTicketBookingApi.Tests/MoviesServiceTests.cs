using Domain.Shared.Constants;
using Moq;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.Movies;
using MovieTicketBookingApi.Tests.Fixtures;
using Xunit;
using DomainAgeRating = Domain.Enums.AgeRating;
using Movie = Domain.Entities.Movie;

namespace MovieTicketBookingApi.Tests;

public class MoviesServiceTests(MoviesServiceFixture fixture) : IClassFixture<MoviesServiceFixture>
{
	[Fact]
	public async Task GetAll_Should_ReturnGetAllMoviesReplyFromCache_WhenRequestIsValid()
	{
        // Arrange
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();

        List<Movie> movies = [
            new()
            {
                Id = firstId,
                Title = "The Phantom's Adventures",
                DurationInMinutes = 124,
                AgeRating = DomainAgeRating.PG13,
                MovieSessions = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                        MovieId = firstId,
                        MovieHallId = Guid.NewGuid(),
                        Tickets = []
                    }
                ]
            },
            new()
            {
                Id = secondId,
                Title = "Tricking Ghosts",
                DurationInMinutes = 98,
                AgeRating = DomainAgeRating.R,
                MovieSessions = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        DateTime = new DateTime(2026, 10, 18, 10, 40, 0, DateTimeKind.Utc),
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
            $"{CacheConstants.MoviesPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";

        fixture.CacheHelper
            .Setup(h => h.Get<IList<Movie>>(cacheKey))
            .Returns(movies);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetAll(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Movies);
        Assert.True(result.Movies.All(h => h.MovieSessions.Count > 0));
	}

	[Fact]
	public async Task GetAll_Should_ReturnGetAllMoviesReplyFromRepository_WhenRequestIsValid()
	{
		// Arrange
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();
        var pageNumber = 1;
        var pageSize = 5;

        List<Movie> movies = [
            new()
            {
                Id = firstId,
                Title = "The Phantom's Adventures",
                DurationInMinutes = 124,
                AgeRating = DomainAgeRating.PG13,
                MovieSessions = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                        MovieId = firstId,
                        MovieHallId = Guid.NewGuid(),
                        Tickets = []
                    }
                ]
            },
            new()
            {
                Id = secondId,
                Title = "Tricking Ghosts",
                DurationInMinutes = 98,
                AgeRating = DomainAgeRating.R,
                MovieSessions = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        DateTime = new DateTime(2026, 10, 18, 10, 40, 0, DateTimeKind.Utc),
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
            $"{CacheConstants.MoviesPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";

        fixture.CacheHelper
            .Setup(h => h.Get<IList<Movie>>(cacheKey))
            .Returns((IList<Movie>)null!);

        fixture.Repository
            .Setup(h => h.GetAllAsync(pageNumber, pageSize))
            .ReturnsAsync(movies);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetAll(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Movies);
        Assert.True(result.Movies.All(h => h.MovieSessions.Count > 0));
	}

	[Fact]
	public async Task GetById_Should_ReturnGetMovieByIdReplyFromCache_WhenMovieExists()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        Movie movie = new()
        {
            Id = id,
            Title = "The Phantom's Adventures",
            DurationInMinutes = 124,
            AgeRating = DomainAgeRating.PG13,
            MovieSessions = [
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                    MovieId = id,
                    MovieHallId = Guid.NewGuid(),
                    Tickets = []
                }
            ]
        };

        GetMovieByIdRequest request = new()
        {
            Id = idString
        };

        var cacheKey = $"{CacheConstants.MoviesPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<Movie>(cacheKey))
            .Returns(movie);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(idString, result.Id);
        Assert.Single(result.MovieSessions);
	}

	[Fact]
	public async Task GetById_Should_ReturnGetMovieByIdReplyFromRepository_WhenMovieExists()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        Movie movie = new()
        {
            Id = id,
            Title = "The Phantom's Adventures",
            DurationInMinutes = 124,
            AgeRating = DomainAgeRating.PG13,
            MovieSessions = [
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                    MovieId = id,
                    MovieHallId = Guid.NewGuid(),
                    Tickets = []
                }
            ]
        };

        GetMovieByIdRequest request = new()
        {
            Id = idString
        };

        var cacheKey = $"{CacheConstants.MoviesPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<Movie>(cacheKey))
            .Returns((Movie)null!);

        fixture.Repository
            .Setup(h => h.GetByIdAsync(idString))
            .ReturnsAsync(movie);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(idString, result.Id);
        Assert.Single(result.MovieSessions);
	}

	[Fact]
	public async Task GetById_Should_ThrowNullReferenceException_WhenMovieDoesNotExist()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        Movie movie = new()
        {
            Id = id,
            Title = "The Phantom's Adventures",
            DurationInMinutes = 124,
            AgeRating = DomainAgeRating.PG13,
            MovieSessions = [
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                    MovieId = id,
                    MovieHallId = Guid.NewGuid(),
                    Tickets = []
                }
            ]
        };

        GetMovieByIdRequest request = new()
        {
            Id = Guid.NewGuid().ToString()
        };

        var cacheKey = $"{CacheConstants.MoviesPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<Movie>(cacheKey))
            .Returns((Movie)null!);

        fixture.Repository
            .Setup(h => h.GetByIdAsync(idString))
            .ReturnsAsync(movie);

        // Act
        var sut = fixture.CreateSut();
        Task<GetMovieByIdReply> result() => sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        await Assert.ThrowsAsync<NullReferenceException>(result);
	}

	[Fact]
	public async Task Create_Should_ReturnCreateMovieReply_WhenRequestIsValid()
	{
		// Arrange
        CreateMovieRequest request = new()
        {
            Title = "The Phantom's Adventures",
            DurationInMinutes = 124,
            AgeRating = AgeRating.Pg13,
        };

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.Create(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotEmpty(result.Id);
        Assert.Equal("The Phantom's Adventures", result.Title);
	}

	[Fact]
	public async Task Update_Should_ReturnEmptyReply_WhenMovieExists()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        Movie movie = new()
        {
            Id = id,
            Title = "The Phantom's Adventures",
            DurationInMinutes = 124,
            AgeRating = DomainAgeRating.PG13,
            MovieSessions = [
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                    MovieId = id,
                    MovieHallId = Guid.NewGuid(),
                    Tickets = []
                }
            ]
        };

        UpdateMovieRequest request = new()
        {
            Id = idString,
            Title = "Tricking Ghosts",
            DurationInMinutes = 98,
            AgeRating = AgeRating.R,
        };

        fixture.Repository
            .Setup(r => r.GetByIdAsync(idString))
            .ReturnsAsync(movie);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.Update(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<EmptyReply>(result);
	}

	[Fact]
	public async Task Update_Should_ThrowNullReferenceException_WhenMovieDoesNotExist()
	{
		// Arrange
        var id = Guid.NewGuid();

        Movie movie = new()
        {
            Id = id,
            Title = "The Phantom's Adventures",
            DurationInMinutes = 124,
            AgeRating = DomainAgeRating.PG13,
            MovieSessions = [
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                    MovieId = id,
                    MovieHallId = Guid.NewGuid(),
                    Tickets = []
                }
            ]
        };

        UpdateMovieRequest request = new()
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Tricking Ghosts",
            DurationInMinutes = 98,
            AgeRating = AgeRating.R,
        };

        fixture.Repository
            .Setup(r => r.GetByIdAsync(id.ToString()))
            .ReturnsAsync(movie);

        // Act
        var sut = fixture.CreateSut();
        Task<EmptyReply> result() => sut.Update(request, fixture.ServerCallContext.Object);

        // Assert
        await Assert.ThrowsAnyAsync<NullReferenceException>(result);
	}

	[Fact]
	public async Task Delete_Should_ReturnEmptyReply_WhenMovieExists()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        Movie movie = new()
        {
            Id = id,
            Title = "The Phantom's Adventures",
            DurationInMinutes = 124,
            AgeRating = DomainAgeRating.PG13,
            MovieSessions = [
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                    MovieId = id,
                    MovieHallId = Guid.NewGuid(),
                    Tickets = []
                }
            ]
        };

        DeleteMovieRequest request = new()
        {
            Id = idString
        };

        fixture.Repository
            .Setup(r => r.GetByIdAsync(idString))
            .ReturnsAsync(movie);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.Delete(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<EmptyReply>(result);
	}

	[Fact]
	public async Task Delete_Should_ThrowNullReferenceException_WhenMovieDoesNotExist()
	{
		// Arrange
        var id = Guid.NewGuid();

        Movie movie = new()
        {
            Id = id,
            Title = "The Phantom's Adventures",
            DurationInMinutes = 124,
            AgeRating = DomainAgeRating.PG13,
            MovieSessions = [
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = new DateTime(2026, 10, 18, 14, 25, 0, DateTimeKind.Utc),
                    MovieId = id,
                    MovieHallId = Guid.NewGuid(),
                    Tickets = []
                }
            ]
        };

        DeleteMovieRequest request = new()
        {
            Id = Guid.NewGuid().ToString()
        };

        fixture.Repository
            .Setup(r => r.GetByIdAsync(id.ToString()))
            .ReturnsAsync(movie);

        // Act
        var sut = fixture.CreateSut();
        Task<EmptyReply> result() => sut.Delete(request, fixture.ServerCallContext.Object);

        // Assert
        await Assert.ThrowsAnyAsync<NullReferenceException>(result);
	}
}
