using Core.Exceptions;
using FluentAssertions;
using Moq;
using MovieTicketBookingApi.Protos.V1.Users;
using MovieTicketBookingApi.Tests.Fixtures;
using Xunit;
using User = Core.Entities.User;

namespace MovieTicketBookingApi.Tests;

public class UsersServiceTests : IClassFixture<UsersServiceFixture>
{
    private readonly UsersServiceFixture _fixture;

    public UsersServiceTests(UsersServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAll_Should_ReturnGetAllUsersReplyFromCache_WhenRequestIsValid()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<IList<User>>(It.IsAny<string>()))
            .Returns(_fixture.Users);

        // Act
        var result = await _fixture.UsersService.GetAll(_fixture.GetPaginatedDataRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetAllUsersReply>();
        result.Users.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_Should_ReturnGetAllUsersReplyFromRepository_WhenRequestIsValid()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<IList<User>>(It.IsAny<string>()))
            .Returns((IList<User>)null!);

        _fixture.UsersRepository
            .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(_fixture.Users);

        // Act
        var result = await _fixture.UsersService.GetAll(_fixture.GetPaginatedDataRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetAllUsersReply>();
        result.Users.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Should_ReturnGetUserByIdReplyFromCache_WhenUserExists()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<User>(It.IsAny<string>()))
            .Returns(_fixture.User);

        // Act
        var result = await _fixture.UsersService.GetById(_fixture.GetUserByIdRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetUserbyIdReply>();
        result.Tickets.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Should_ReturnGetUserByIdReplyFromRepository_WhenUserExists()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<User>(It.IsAny<string>()))
            .Returns((User)null!);

        _fixture.UsersRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.User);

        // Act
        var result = await _fixture.UsersService.GetById(_fixture.GetUserByIdRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<GetUserbyIdReply>();
        result.Tickets.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetById_Should_ThrowNullReferenceException_WhenTicketDoesNotExist()
    {
        // Arrange
        _fixture.CacheHelper
            .Setup(h => h.Get<User>(It.IsAny<string>()))
            .Returns((User)null!);

        _fixture.UsersRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        // Act
        var result = async () => await _fixture.UsersService.GetById(_fixture.GetUserByIdRequest, _fixture.ServerCallContext);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task Register_Should_ReturnRegisterUserReply_WhenRequestIsValid()
    {
        // Act
        var result = await _fixture.UsersService.Register(_fixture.RegisterUserRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<RegisterUserReply>();
    }

    [Fact]
    public async Task Login_Should_ReturnLoginUserReply_WhenUserExists()
    {
        // Arrange
        _fixture.UsersRepository
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.User);

        // Act
        var result = await _fixture.UsersService.Login(_fixture.LoginUserRequest, _fixture.ServerCallContext);

        // Assert
        result.Should().NotBeNull().And.BeOfType<LoginUserReply>();
    }

    [Fact]
    public async Task Login_Should_ThrowNullReferenceException_WhenUserDoesNotExist()
    {
        // Arrange
        _fixture.UsersRepository
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        // Act
        var result = async () => await _fixture.UsersService.Login(_fixture.LoginUserRequest, _fixture.ServerCallContext);

        // Assert
        await result.Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task Login_Should_ThrowIncorrectPasswordException_WhenPasswordIsIncorrect()
    {
        // Arrange
        _fixture.UsersRepository
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.User);

        // Act
        var result = async () => await _fixture.UsersService.Login(_fixture.LoginUserRequest, _fixture.ServerCallContext);

        // Assert
        await result.Should().ThrowAsync<IncorrectPasswordException>();
    }
}
