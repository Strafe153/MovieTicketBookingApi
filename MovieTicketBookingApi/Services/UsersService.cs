using AutoMapper;
using Core.Interfaces.Repositories;
using Grpc.Core;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.Users;

namespace MovieTicketBookingApi.Services;

public class UsersService : Users.UsersBase
{
    private readonly IUsersRepository _repository;
    private readonly IMapper _mapper;

    public UsersService(
        IUsersRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public override async Task<GetAllUsersReply> GetAll(GetPaginatedDataRequest request, ServerCallContext context) =>
        _mapper.Map<GetAllUsersReply>(await _repository.GetAllAsync(request.PageNumber, request.PageSize));

    public override async Task<GetUserbyIdReply> GetById(GetUserByIdRequest request, ServerCallContext context)
    {
        var user = await _repository.GetByIdAsync(new Guid(request.Id));

        if (user is null)
        {
            throw new NullReferenceException();
        }

        return _mapper.Map<GetUserbyIdReply>(user);
    }

    public override async Task<EmptyReply> Update(UpdateUserRequest request, ServerCallContext context)
    {
        var user = await _repository.GetByIdAsync(new Guid(request.Id));

        if (user is null)
        {
            throw new NullReferenceException();
        }

        _mapper.Map(request, user);
        await _repository.Update(user);

        return new EmptyReply();
    }

    public override async Task<EmptyReply> Delete(DeleteUserRequest request, ServerCallContext context)
    {
        var user = await _repository.GetByIdAsync(new Guid(request.Id));

        if (user is null)
        {
            throw new NullReferenceException();
        }

        await _repository.Delete(user.Id);

        return new EmptyReply();
    }
}
