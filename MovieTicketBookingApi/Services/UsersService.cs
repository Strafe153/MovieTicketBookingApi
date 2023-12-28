using AutoMapper;
using Core.Extensions;
using Core.Interfaces.Helpers;
using Core.Interfaces.Repositories;
using Core.Shared;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.Users;
using User = Core.Entities.User;

namespace MovieTicketBookingApi.Services;

[Authorize]
[EnableRateLimiting("tokenBucket")]
public class UsersService : Users.UsersBase
{
    private readonly IUsersRepository _repository;
    private readonly IPasswordHelper _passwordHelper;
    private readonly ITokenHelper _tokenHelper;
    private readonly ICacheHelper _cacheHelper;
    private readonly IMapper _mapper;
    private readonly CacheOptions _cacheOptions;

    public UsersService(
        IUsersRepository repository,
        IPasswordHelper passwordHelper,
        ITokenHelper tokenHelper,
        ICacheHelper cacheHelper,
        IMapper mapper,
        CacheOptions cacheOptions)
    {
        _repository = repository;
        _passwordHelper = passwordHelper;
        _tokenHelper = tokenHelper;
        _cacheHelper = cacheHelper;
        _mapper = mapper;
        _cacheOptions = cacheOptions;
    }

    public override async Task<GetAllUsersReply> GetAll(GetPaginatedDataRequest request, ServerCallContext context)
    {
        var key = $"users:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";
        var users = _cacheHelper.Get<IList<User>>(key);

        if (users is null)
        {
            users = await _repository.GetAllAsync(request.PageNumber.Value, request.PageSize.Value);
            _cacheHelper.Set(key, users, _cacheOptions);
        }

        return _mapper.Map<GetAllUsersReply>(users);
    }

    public override async Task<GetUserbyIdReply> GetById(GetUserByIdRequest request, ServerCallContext context)
    {
        var key = $"users:{request.Id}";
        var user = _cacheHelper.Get<User>(key);

        if (user is null)
        {
            user = await _repository.GetByIdOrThrowAsync(request.Id);
            _cacheHelper.Set(key, user, _cacheOptions);
        }

        return _mapper.Map<GetUserbyIdReply>(user);
    }

    [AllowAnonymous]
    public override async Task<RegisterUserReply> Register(RegisterUserRequest request, ServerCallContext context)
    {
        var user = _mapper.Map<User>(request);

        user.Id = Guid.NewGuid();
        (user.PasswordHash, user.PasswordSalt) = _passwordHelper.GeneratePasswordHashAndSalt(request.Password);

        await _repository.InsertAsync(user);

        return _mapper.Map<RegisterUserReply>(user);
    }

    [AllowAnonymous]
    public override async Task<LoginUserReply> Login(LoginUserRequest request, ServerCallContext context)
    {
        var user = await _repository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            throw new NullReferenceException($"User with email {request.Email} does not exist");
        }

        _passwordHelper.VerifyPasswordHash(request.Password, user);
        var accessToken = _tokenHelper.GenerateAccessToken(user);

        return new LoginUserReply
        {
            AccessToken = accessToken
        };
    }

    public override async Task<EmptyReply> Update(UpdateUserRequest request, ServerCallContext context)
    {
        var user = await _repository.GetByIdOrThrowAsync(request.Id);

        _mapper.Map(request, user);
        await _repository.UpdateAsync(user);

        return new EmptyReply();
    }

    public override async Task<EmptyReply> Delete(DeleteUserRequest request, ServerCallContext context)
    {
        await _repository.GetByIdOrThrowAsync(request.Id);
        await _repository.DeleteAsync(request.Id);

        return new EmptyReply();
    }
}
