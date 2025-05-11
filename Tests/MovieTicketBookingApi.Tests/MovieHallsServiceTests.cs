using FluentAssertions;
using Moq;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.V1.MovieHalls;
using MovieTicketBookingApi.Tests.Fixtures;
using Xunit;
using MovieHall = Domain.Entities.MovieHall;

namespace MovieTicketBookingApi.Tests;

public class MovieHallsServiceTests : IClassFixture<MovieHallsServiceFixture>
{
    private readonly MovieHallsServiceFixture _fixture;

    public MovieHallsServiceTests(MovieHallsServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAll_Should_ReturnGetAllMovieHallsReplyFromCache_WhenRequestIsValid()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<IList<MovieHall>>(It.IsAny<string>()))
            .Returns(_fixture.MovieHalls);

        // Act
        var result = await _fixture.MovieHallsService.GetAll(_fixture.GetPaginatedDataRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetAllMovieHallsReply>();
        result.MovieHalls.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_Should_ReturnGetAllMovieHallsReplyFromRepository_WhenRequestIsValid()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<IList<MovieHall>>(It.IsAny<string>()))
            .Returns((IList<MovieHall>)null!);

        _fixture.MovieHallsRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(_fixture.MovieHalls);

        // Act
        var result = await _fixture.MovieHallsService.GetAll(_fixture.GetPaginatedDataRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetAllMovieHallsReply>();
        result.MovieHalls.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Should_ReturnGetMovieHallByIdReplyFromCache_WhenMovieHallExists()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<MovieHall>(It.IsAny<string>()))
            .Returns(_fixture.MovieHall);

        // Act
        var result = await _fixture.MovieHallsService.GetById(_fixture.GetMovieHallByIdRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetMovieHallByIdReply>();
    }

    [Fact]
    public async Task GetById_Should_ReturnGetMovieHallByIdReplyFromRepository_WhenMovieHallExists()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<MovieHall>(It.IsAny<string>()))
            .Returns((MovieHall)null!);

        _fixture.MovieHallsRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.MovieHall);

        // Act
        var result = await _fixture.MovieHallsService.GetById(_fixture.GetMovieHallByIdRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetMovieHallByIdReply>();
    }

    [Fact]
    public async Task GetById_Should_ThrowNullReferenceException_WhenMovieHallDoesNotExist()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<MovieHall>(It.IsAny<string>()))
            .Returns((MovieHall)null!);

        _fixture.MovieHallsRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((MovieHall)null!);

        // Act
        var result = () => _fixture.MovieHallsService.GetById(_fixture.GetMovieHallByIdRequest, _fixture.ServerCallContext);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task Create_Should_ReturnCreateMovieHallReply_WhenRequestIsValid()
    {
        // Act
        var result = await _fixture.MovieHallsService.Create(_fixture.CreateMovieHallRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<CreateMovieHallReply>();
    }

    [Fact]
    public async Task Update_Should_ReturnEmptyReply_WhenMovieHallExists()
    {
        // Arrange
        _fixture.MovieHallsRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.MovieHall);

        // Act
        var result = await _fixture.MovieHallsService.Update(_fixture.UpdateMovieHallRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<EmptyReply>();
    }

    [Fact]
    public async Task Update_Should_ThrowNullReferenceException_WhenMovieHallDoesNotExist()
    {
        // Arrange
        _fixture.MovieHallsRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((MovieHall)null!);

        // Act
        var result = () => _fixture.MovieHallsService.Update(_fixture.UpdateMovieHallRequest, _fixture.ServerCallContext);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task Delete_Should_ReturnEmptyReply_WhenMovieHallExists()
    {
        // Arrange
        _fixture.MovieHallsRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.MovieHall);

        // Act
        var result = await _fixture.MovieHallsService.Delete(_fixture.DeleteMovieHallRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<EmptyReply>();
    }

    [Fact]
    public async Task Delete_Should_ThrowNullReferenceException_WhenMovieHallDoesNotExist()
    {
        // Arrange
        _fixture.MovieHallsRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((MovieHall)null!);

        // Act
        var result = () => _fixture.MovieHallsService.Delete(_fixture.DeleteMovieHallRequest, _fixture.ServerCallContext);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }
}
