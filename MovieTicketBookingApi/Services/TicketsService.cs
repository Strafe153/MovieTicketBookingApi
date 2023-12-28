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
using MovieTicketBookingApi.Protos.V1.Tickets;
using Ticket = Core.Entities.Ticket;

namespace MovieTicketBookingApi.Services;

[Authorize]
[EnableRateLimiting("tokenBucket")]
public class TicketsService : Tickets.TicketsBase
{
    private readonly ITicketsRepository _repository;
    private readonly ICacheHelper _cacheHelper;
    private readonly IMapper _mapper;
    private readonly CacheOptions _cacheOptions;

    public TicketsService(
        ITicketsRepository repository,
        ICacheHelper cacheHelper,
        IMapper mapper,
        CacheOptions cacheOptions)
    {
        _repository = repository;
        _cacheHelper = cacheHelper;
        _mapper = mapper;
        _cacheOptions = cacheOptions;
    }

    public override async Task<GetTicketsReply> GetAll(GetPaginatedDataRequest request, ServerCallContext context)
    {
        var key = $"tickets:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";
        var tickets = _cacheHelper.Get<IList<Ticket>>(key);

        if (tickets is null)
        {
            tickets = await _repository.GetAllAsync(request.PageNumber.Value, request.PageSize.Value);
            _cacheHelper.Set(key, tickets, _cacheOptions);
        }

        return _mapper.Map<GetTicketsReply>(tickets);
    }

    public override async Task<GetTicketByIdReply> GetById(GetTicketByIdRequest request, ServerCallContext context)
    {
        var key = $"tickets:{request.Id}";
        var ticket = _cacheHelper.Get<Ticket>(key);

        if (ticket is null)
        {
            ticket = await _repository.GetByIdOrThrowAsync(request.Id);
            _cacheHelper.Set(key, ticket, _cacheOptions);
        }

        return _mapper.Map<GetTicketByIdReply>(ticket);
    }

    public override async Task<GetTicketsReply> GetByUserId(GetTicketsByUserIdRequest request, ServerCallContext context)
    {
        var key = $"tickets:{request.PageNumber ??= 1}{request.PageSize ??= 5}:{request.UserId}";
        var tickets = _cacheHelper.Get<IList<Ticket>>(key);

        if (tickets is null)
        {
            tickets = await _repository.GetByUserIdAsync(request.PageNumber.Value, request.PageSize.Value, request.UserId);
            _cacheHelper.Set(key, tickets, _cacheOptions);
        }

        return _mapper.Map<GetTicketsReply>(tickets);
    }

    public override async Task<CreateTicketReply> Create(CreateTicketRequest request, ServerCallContext context)
    {
        var ticket = _mapper.Map<Ticket>(request);
        ticket.Id = Guid.NewGuid();

        await _repository.InsertAsync(ticket);

        return _mapper.Map<CreateTicketReply>(ticket);
    }

    public override async Task<EmptyReply> Delete(DeleteTicketRequest request, ServerCallContext context)
    {
        await _repository.GetByIdOrThrowAsync(request.Id);
        await _repository.DeleteAsync(request.Id);

        return new EmptyReply();
    }
}
