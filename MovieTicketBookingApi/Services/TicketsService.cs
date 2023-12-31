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
    private readonly ITicketsRepository _ticketsRepository;
    private readonly IMovieSessionsRepository _movieSessionsRepository;
    private readonly ICacheHelper _cacheHelper;
    private readonly IMapper _mapper;
    private readonly CacheOptions _cacheOptions;

    public TicketsService(
        ITicketsRepository ticketsRepository,
        IMovieSessionsRepository movieSessionsRepository,
        ICacheHelper cacheHelper,
        IMapper mapper,
        CacheOptions cacheOptions)
    {
        _ticketsRepository = ticketsRepository;
        _movieSessionsRepository = movieSessionsRepository;
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
            tickets = await _ticketsRepository.GetAllAsync(request.PageNumber.Value, request.PageSize.Value);
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
            ticket = await _ticketsRepository.GetByIdOrThrowAsync(request.Id);
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
            tickets = await _ticketsRepository.GetByUserIdAsync(request.PageNumber.Value, request.PageSize.Value, request.UserId);
            _cacheHelper.Set(key, tickets, _cacheOptions);
        }

        return _mapper.Map<GetTicketsReply>(tickets);
    }

    public override async Task<CreateTicketReply> Create(CreateTicketRequest request, ServerCallContext context)
    {
        var movieSession = await _movieSessionsRepository.GetByIdOrThrowAsync(request.MovieSessionId);

        var ticket = _mapper.Map<Ticket>(request);
        ticket.DateTime = movieSession.DateTime;
        ticket.Id = Guid.NewGuid();

        await _ticketsRepository.InsertAsync(ticket);

        return _mapper.Map<CreateTicketReply>(ticket);
    }

    public override async Task<EmptyReply> Delete(DeleteTicketRequest request, ServerCallContext context)
    {
        await _ticketsRepository.GetByIdOrThrowAsync(request.Id);
        await _ticketsRepository.DeleteAsync(request.Id);

        return new EmptyReply();
    }
}
