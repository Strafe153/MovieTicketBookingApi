using FluentAssertions;
using Moq;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.V1.Movies;
using MovieTicketBookingApi.Tests.Fixtures;
using Xunit;
using Movie = Core.Entities.Movie;

namespace MovieTicketBookingApi.Tests;

public class MoviesServiceTests : IClassFixture<MoviesServiceFixture>
{
    private readonly MoviesServiceFixture _fixture;

    public MoviesServiceTests(MoviesServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAll_Should_ReturnGetAllMoviesReplyFromCache_WhenRequestIsValid()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<IList<Movie>>(It.IsAny<string>()))
            .Returns(_fixture.Movies);

        // Act
        var result = await _fixture.MoviesService.GetAll(_fixture.GetPaginatedDataRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetAllMoviesReply>();
        result.Movies.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_Should_ReturnGetAllMoviesReplyFromRepository_WhenRequestIsValid()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<IList<Movie>>(It.IsAny<string>()))
            .Returns((IList<Movie>)null!);

        _fixture.MoviesRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(_fixture.Movies);

        // Act
        var result = await _fixture.MoviesService.GetAll(_fixture.GetPaginatedDataRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetAllMoviesReply>();
        result.Movies.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Should_ReturnGetMovieByIdReplyFromCache_WhenMovieExists()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<Movie>(It.IsAny<string>()))
            .Returns(_fixture.Movie);

        // Act
        var result = await _fixture.MoviesService.GetById(_fixture.GetMovieByIdRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetMovieByIdReply>();
    }

    [Fact]
    public async Task GetById_Should_ReturnGetMovieByIdReplyFromRepository_WhenMovieExists()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<Movie>(It.IsAny<string>()))
            .Returns((Movie)null!);

        _fixture.MoviesRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Movie);

        // Act
        var result = await _fixture.MoviesService.GetById(_fixture.GetMovieByIdRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetMovieByIdReply>();
    }

    [Fact]
    public async Task GetById_Should_ThrowNullReferenceException_WhenMovieDoesNotExist()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<Movie>(It.IsAny<string>()))
            .Returns((Movie)null!);

        _fixture.MoviesRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Movie)null!);

        // Act
        var result = async () => await _fixture.MoviesService.GetById(_fixture.GetMovieByIdRequest, _fixture.ServerCallContext);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task Create_Should_ReturnCreateMovieReply_WhenRequestIsValid()
    {
        // Act
        var result = await _fixture.MoviesService.Create(_fixture.CreateMovieRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<CreateMovieReply>();
    }

    [Fact]
    public async Task Update_Should_ReturnEmptyReply_WhenMovieExists()
    {
        // Arrange
        _fixture.MoviesRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Movie);

        // Act
        var result = await _fixture.MoviesService.Update(_fixture.UpdateMovieRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<EmptyReply>();
    }

    [Fact]
    public async Task Update_Should_ThrowNullReferenceException_WhenMovieDoesNotExist()
    {
        // Arrange
        _fixture.MoviesRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Movie)null!);

        // Act
        var result = async () => await _fixture.MoviesService.Update(_fixture.UpdateMovieRequest, _fixture.ServerCallContext);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task Delete_Should_ReturnEmptyReply_WhenMovieExists()
    {
        // Arrange
        _fixture.MoviesRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Movie);

        // Act
        var result = await _fixture.MoviesService.Delete(_fixture.DeleteMovieRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<EmptyReply>();
    }

    [Fact]
    public async Task Delete_Should_ThrowNullReferenceException_WhenMovieDoesNotExist()
    {
        // Arrange
        _fixture.MoviesRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((Movie)null!);

        // Act
        var result = async () => await _fixture.MoviesService.Delete(_fixture.DeleteMovieRequest, _fixture.ServerCallContext);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }
}
