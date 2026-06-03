using System.Security.Cryptography;
using Domain.Entities;
using Domain.Shared;
using Domain.Shared.Constants;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using Moq;
using MovieTicketBookingApi.Helpers;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.Users;
using MovieTicketBookingApi.Tests.Fixtures;
using Xunit;
using DateTime = System.DateTime;
using User = Domain.Entities.User;

namespace MovieTicketBookingApi.Tests;

public class UsersServiceTests(UsersServiceFixture fixture) : IClassFixture<UsersServiceFixture>
{
	[Fact]
	public async Task GetAll_Should_ReturnGetAllUsersReplyFromCache_WhenRequestIsValid()
	{
		// Arrange
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();

        List<User> users = [
            new()
            {
                Id = firstId,
                FirstName = "Ryuji",
                LastName = "Sakamoto",
                Email = "phantom_skull@thieves.com",
                BirthDate = new DateTime(2000, 04, 06, 0, 0, 0, DateTimeKind.Utc),
                Tickets = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        SeatNumber = 1,
                        MovieSessionId = Guid.NewGuid(),
                        UserId = firstId
                    }
                ]
            },
            new()
            {
                Id = secondId,
                FirstName = "Yusuke",
                LastName = "Kitagawa",
                Email = "fox@thieves.com",
                BirthDate = new DateTime(2000, 08, 30, 0, 0, 0, DateTimeKind.Utc),
                Tickets = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        SeatNumber = 1,
                        MovieSessionId = Guid.NewGuid(),
                        UserId = secondId
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
            $"{CacheConstants.UsersPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";

        fixture.CacheHelper
            .Setup(h => h.Get<IList<User>>(cacheKey))
            .Returns(users);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetAll(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Users);
        Assert.True(result.Users.All(h => h.Tickets.Count > 0));
	}

	[Fact]
	public async Task GetAll_Should_ReturnGetAllUsersReplyFromRepository_WhenRequestIsValid()
	{
		// Arrange
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();
        var pageNumber = 1;
        var pageSize = 5;

        List<User> users = [
            new()
            {
                Id = firstId,
                FirstName = "Ryuji",
                LastName = "Sakamoto",
                Email = "phantom_skull@thieves.com",
                BirthDate = new DateTime(2000, 04, 06, 0, 0, 0, DateTimeKind.Utc),
                Tickets = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        SeatNumber = 1,
                        MovieSessionId = Guid.NewGuid(),
                        UserId = firstId
                    }
                ]
            },
            new()
            {
                Id = secondId,
                FirstName = "Yusuke",
                LastName = "Kitagawa",
                Email = "fox@thieves.com",
                BirthDate = new DateTime(2000, 08, 30, 0, 0, 0, DateTimeKind.Utc),
                Tickets = [
                    new()
                    {
                        Id = Guid.NewGuid(),
                        SeatNumber = 1,
                        MovieSessionId = Guid.NewGuid(),
                        UserId = secondId
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
            $"{CacheConstants.UsersPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";

        fixture.CacheHelper
            .Setup(h => h.Get<IList<User>>(cacheKey))
            .Returns((IList<User>)null!);

        fixture.Repository
            .Setup(h => h.GetAllAsync(pageNumber, pageSize))
            .ReturnsAsync(users);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetAll(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Users);
        Assert.True(result.Users.All(h => h.Tickets.Count > 0));
	}

	[Fact]
	public async Task GetById_Should_ReturnGetUserByIdReplyFromCache_WhenUserExists()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        User user = new()
        {
            Id = id,
            FirstName = "Ryuji",
            LastName = "Sakamoto",
            Email = "phantom_skull@thieves.com",
            BirthDate = new DateTime(2000, 04, 06, 0, 0, 0, DateTimeKind.Utc),
            Tickets = [
                new()
                {
                    Id = Guid.NewGuid(),
                    SeatNumber = 1,
                    MovieSessionId = Guid.NewGuid(),
                    UserId = id
                }
            ]
        };

        GetUserByIdRequest request = new()
        {
            Id = idString
        };

        var cacheKey = $"{CacheConstants.UsersPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<User>(cacheKey))
            .Returns(user);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(idString, result.Id);
        Assert.Single(result.Tickets);
	}

	[Fact]
	public async Task GetById_Should_ReturnGetUserByIdReplyFromRepository_WhenUserExists()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        User user = new()
        {
            Id = id,
            FirstName = "Ryuji",
            LastName = "Sakamoto",
            Email = "phantom_skull@thieves.com",
            BirthDate = new DateTime(2000, 04, 06, 0, 0, 0, DateTimeKind.Utc),
            Tickets = [
                new()
                {
                    Id = Guid.NewGuid(),
                    SeatNumber = 1,
                    MovieSessionId = Guid.NewGuid(),
                    UserId = id
                }
            ]
        };

        GetUserByIdRequest request = new()
        {
            Id = idString
        };

        var cacheKey = $"{CacheConstants.UsersPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<User>(cacheKey))
            .Returns((User)null!);

        fixture.Repository
            .Setup(h => h.GetByIdAsync(idString))
            .ReturnsAsync(user);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(idString, result.Id);
        Assert.Single(result.Tickets);
	}

	[Fact]
	public async Task GetById_Should_ThrowNullReferenceException_WhenTicketDoesNotExist()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        User user = new()
        {
            Id = id,
            FirstName = "Ryuji",
            LastName = "Sakamoto",
            Email = "phantom_skull@thieves.com",
            BirthDate = new DateTime(2000, 04, 06, 0, 0, 0, DateTimeKind.Utc),
            Tickets = [
                new()
                {
                    Id = Guid.NewGuid(),
                    SeatNumber = 1,
                    MovieSessionId = Guid.NewGuid(),
                    UserId = id
                }
            ]
        };

        GetUserByIdRequest request = new()
        {
            Id = Guid.NewGuid().ToString()
        };

        var cacheKey = $"{CacheConstants.UsersPrefix}:{request.Id}";

        fixture.CacheHelper
            .Setup(h => h.Get<User>(cacheKey))
            .Returns((User)null!);

        fixture.Repository
            .Setup(h => h.GetByIdAsync(idString))
            .ReturnsAsync(user);

        // Act
        var sut = fixture.CreateSut();
        Task<GetUserbyIdReply> result() => sut.GetById(request, fixture.ServerCallContext.Object);

        // Assert
        await Assert.ThrowsAsync<NullReferenceException>(result);
    }

	[Fact]
	public async Task Register_Should_ReturnRegisterUserReply_WhenRequestIsValid()
	{
        // Arrange
        RegisterUserRequest request = new()
        {
            FirstName = "Ryuji",
            LastName = "Sakamoto",
            Email = "phantom_skull@thieves.com",
            BirthDate = Timestamp.FromDateTime(new DateTime(2000, 04, 06, 0, 0, 0, DateTimeKind.Utc)),
            Password = "qWer5678*("
        };

		// Act
        var sut = fixture.CreateSut();
		var result = await sut.Register(request, fixture.ServerCallContext.Object);

		// Assert
        Assert.NotNull(result);
        Assert.Equal("Ryuji", result.FirstName);
        Assert.Equal("phantom_skull@thieves.com", result.Email);
	}

	[Fact]
	public async Task Login_Should_ReturnLoginUserReply_WhenUserExists()
	{
		// Arrange
        var id = Guid.NewGuid();
        var email = "phantom_skull@thieves.com";
        var password = "qWer5678*(";

        PasswordHelper passwordHelper = new();
        var (hash, salt) = passwordHelper.GeneratePasswordHashAndSalt(password);

        User user = new()
        {
            Id = id,
            FirstName = "Ryuji",
            LastName = "Sakamoto",
            Email = email,
            BirthDate = new DateTime(2000, 04, 06, 0, 0, 0, DateTimeKind.Utc),
            PasswordHash = hash,
            PasswordSalt = salt,
            Tickets = [
                new()
                {
                    Id = Guid.NewGuid(),
                    SeatNumber = 1,
                    MovieSessionId = Guid.NewGuid(),
                    UserId = id
                }
            ]
        };

        LoginUserRequest request = new()
        {
            Email = email,
            Password = password
        };

		fixture.Repository
			.Setup(r => r.GetByEmailAsync(email))
			.ReturnsAsync(user);

        fixture.TokenHelper
            .Setup(h => h.GenerateAccessToken(user))
            .Returns(Guid.NewGuid().ToString());

		// Act
        var sut = fixture.CreateSut();
		var result = await sut.Login(request, fixture.ServerCallContext.Object);

		// Assert
		Assert.NotEmpty(result.AccessToken);
	}

	[Fact]
	public async Task Login_Should_ThrowNullReferenceException_WhenUserDoesNotExist()
	{
		// Arrange
        var id = Guid.NewGuid();
        var email = "phantom_skull@thieves.com";
        var password = "qWer5678*(";

        PasswordHelper passwordHelper = new();
        var (hash, salt) = passwordHelper.GeneratePasswordHashAndSalt(password);

        User user = new()
        {
            Id = id,
            FirstName = "Ryuji",
            LastName = "Sakamoto",
            Email = email,
            BirthDate = new DateTime(2000, 04, 06, 0, 0, 0, DateTimeKind.Utc),
            PasswordHash = hash,
            PasswordSalt = salt,
            Tickets = [
                new()
                {
                    Id = Guid.NewGuid(),
                    SeatNumber = 1,
                    MovieSessionId = Guid.NewGuid(),
                    UserId = id
                }
            ]
        };

        LoginUserRequest request = new()
        {
            Email = "fox@thieves.com",
            Password = password
        };

		fixture.Repository
			.Setup(r => r.GetByEmailAsync(email))
			.ReturnsAsync(user);

		// Act
        var sut = fixture.CreateSut();
        Task<LoginUserReply> result() => sut.Login(request, fixture.ServerCallContext.Object);

        // Assert
        await Assert.ThrowsAsync<NullReferenceException>(result);
    }

	[Fact]
	public async Task Update_Should_ReturnEmptyReply_WhenUserExists()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        User user = new()
        {
            Id = id,
            FirstName = "Ryuji",
            LastName = "Sakamoto",
            Email = "phantom_skull@thieves.com",
            BirthDate = new DateTime(2000, 04, 06, 0, 0, 0, DateTimeKind.Utc),
            Tickets = [
                new()
                {
                    Id = Guid.NewGuid(),
                    SeatNumber = 1,
                    MovieSessionId = Guid.NewGuid(),
                    UserId = id
                }
            ]
        };

        UpdateUserRequest request = new()
        {
            Id = idString,
            FirstName = "Yusuke",
            LastName = "Kitagawa",
            BirthDate = Timestamp.FromDateTime(new DateTime(2000, 08, 30, 0, 0, 0, DateTimeKind.Utc))
        };

        fixture.Repository
            .Setup(r => r.GetByIdAsync(idString))
            .ReturnsAsync(user);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.Update(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<EmptyReply>(result);
	}

	[Fact]
	public async Task Update_Should_ThrowNullReferenceException_WhenUserDoesNotExist()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        User user = new()
        {
            Id = id,
            FirstName = "Ryuji",
            LastName = "Sakamoto",
            Email = "phantom_skull@thieves.com",
            BirthDate = new DateTime(2000, 04, 06, 0, 0, 0, DateTimeKind.Utc),
            Tickets = [
                new()
                {
                    Id = Guid.NewGuid(),
                    SeatNumber = 1,
                    MovieSessionId = Guid.NewGuid(),
                    UserId = id
                }
            ]
        };

        UpdateUserRequest request = new()
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "Yusuke",
            LastName = "Kitagawa",
            BirthDate = Timestamp.FromDateTime(new DateTime(2000, 08, 30, 0, 0, 0, DateTimeKind.Utc))
        };

        fixture.Repository
            .Setup(r => r.GetByIdAsync(idString))
            .ReturnsAsync(user);

        // Act
        var sut = fixture.CreateSut();
        Task<EmptyReply> result() => sut.Update(request, fixture.ServerCallContext.Object);

        // Assert
        await Assert.ThrowsAsync<NullReferenceException>(result);
    }

	[Fact]
	public async Task Delete_Should_ReturnEmptyReply_WhenUserExists()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        User user = new()
        {
            Id = id,
            FirstName = "Ryuji",
            LastName = "Sakamoto",
            Email = "phantom_skull@thieves.com",
            BirthDate = new DateTime(2000, 04, 06, 0, 0, 0, DateTimeKind.Utc),
            Tickets = [
                new()
                {
                    Id = Guid.NewGuid(),
                    SeatNumber = 1,
                    MovieSessionId = Guid.NewGuid(),
                    UserId = id
                }
            ]
        };

        DeleteUserRequest request = new()
        {
            Id = idString
        };

        fixture.Repository
            .Setup(r => r.GetByIdAsync(idString))
            .ReturnsAsync(user);

        // Act
        var sut = fixture.CreateSut();
        var result = await sut.Delete(request, fixture.ServerCallContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<EmptyReply>(result);
	}

	[Fact]
	public async Task Delete_Should_ThrowNullReferenceException_WhenUserDoesNotExist()
	{
		// Arrange
        var id = Guid.NewGuid();
        var idString = id.ToString();

        User user = new()
        {
            Id = id,
            FirstName = "Ryuji",
            LastName = "Sakamoto",
            Email = "phantom_skull@thieves.com",
            BirthDate = new DateTime(2000, 04, 06, 0, 0, 0, DateTimeKind.Utc),
            Tickets = [
                new()
                {
                    Id = Guid.NewGuid(),
                    SeatNumber = 1,
                    MovieSessionId = Guid.NewGuid(),
                    UserId = id
                }
            ]
        };

        DeleteUserRequest request = new()
        {
            Id = Guid.NewGuid().ToString()
        };

        fixture.Repository
            .Setup(r => r.GetByIdAsync(idString))
            .ReturnsAsync(user);

        // Act
        var sut = fixture.CreateSut();
        Task<EmptyReply> result() => sut.Delete(request, fixture.ServerCallContext.Object);

        // Assert
        await Assert.ThrowsAnyAsync<NullReferenceException>(result);
	}
}
