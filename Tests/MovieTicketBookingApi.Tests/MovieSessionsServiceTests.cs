using FluentAssertions;
using Moq;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.V1.MovieSessions;
using MovieTicketBookingApi.Tests.Fixtures;
using Xunit;
using MovieSession = Core.Entities.MovieSession;

namespace MovieTicketBookingApi.Tests;

public class MovieSessionsServiceTests : IClassFixture<MovieSessionsServiceFixture>
{
    private readonly MovieSessionsServiceFixture _fixture;

    public MovieSessionsServiceTests(MovieSessionsServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAll_Should_ReturnGetAllMovieSessionsReplyFromCache_WhenRequestIsValid()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<IList<MovieSession>>(It.IsAny<string>()))
            .Returns(_fixture.MovieSessions);

        // Act
        var result = await _fixture.MovieSessionsService.GetAll(_fixture.GetPaginatedDataRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetAllMovieSessionsReply>();
        result.MovieSessions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_Should_ReturnGetAllMovieSessionsReplyFromRepository_WhenRequestIsValid()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<IList<MovieSession>>(It.IsAny<string>()))
            .Returns((IList<MovieSession>)null!);

        _fixture.MovieSessionsRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(_fixture.MovieSessions);

        // Act
        var result = await _fixture.MovieSessionsService.GetAll(_fixture.GetPaginatedDataRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetAllMovieSessionsReply>();
        result.MovieSessions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Should_ReturnGetMovieSessionByIdReplyFromCache_WhenMovieSessionExists()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<MovieSession>(It.IsAny<string>()))
            .Returns(_fixture.MovieSession);

        // Act
        var result = await _fixture.MovieSessionsService.GetById(_fixture.GetMovieSessionByIdRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetMovieSessionByIdReply>();
        result.Tickets.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Should_ReturnGetMovieSessionByIdReplyFromRepository_WhenMovieSessionExists()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<MovieSession>(It.IsAny<string>()))
            .Returns((MovieSession)null!);

        _fixture.MovieSessionsRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.MovieSession);

        // Act
        var result = await _fixture.MovieSessionsService.GetById(_fixture.GetMovieSessionByIdRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetMovieSessionByIdReply>();
        result.Tickets.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Should_ThrowNullReferenceException_WhenMovieSessionDoesNotExist()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<MovieSession>(It.IsAny<string>()))
            .Returns((MovieSession)null!);

        _fixture.MovieSessionsRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((MovieSession)null!);

        // Act
        var result = async () => await _fixture.MovieSessionsService.GetById(_fixture.GetMovieSessionByIdRequest, _fixture.ServerCallContext);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task Create_Should_ReturnCreateMovieSessionReply_WhenRequestIsValid()
    {
        // Act
        var result = await _fixture.MovieSessionsService.Create(_fixture.CreateMovieSessionRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<CreateMovieSessionReply>();
    }

    [Fact]
    public async Task Update_Should_ReturnEmptyReply_WhenMovieSessionExists()
    {
        // Arrange
        _fixture.MovieSessionsRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.MovieSession);

        // Act
        var result = await _fixture.MovieSessionsService.Update(_fixture.UpdateMovieSessionRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<EmptyReply>();
    }

    [Fact]
    public async Task Update_Should_ThrowNullReferenceException_WhenMovieSessionDoesNotExist()
    {
        // Arrange
        _fixture.MovieSessionsRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((MovieSession)null!);

        // Act
        var result = async () => await _fixture.MovieSessionsService.Update(_fixture.UpdateMovieSessionRequest, _fixture.ServerCallContext);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }
}
