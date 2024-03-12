using AutoMapper;
using Domain.Interfaces.Helpers;
using Domain.Interfaces.Repositories;
using Domain.Shared.Constants;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.Tickets;
using Ticket = Domain.Entities.Ticket;

namespace MovieTicketBookingApi.Services;

[Authorize]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class TicketsService : Tickets.TicketsBase
{
	private readonly ITicketsRepository _ticketsRepository;
	private readonly IMovieSessionsRepository _movieSessionsRepository;
	private readonly ICacheHelper _cacheHelper;
	private readonly IMapper _mapper;

	public TicketsService(
		ITicketsRepository ticketsRepository,
		IMovieSessionsRepository movieSessionsRepository,
		ICacheHelper cacheHelper,
		IMapper mapper)
	{
		_ticketsRepository = ticketsRepository;
		_movieSessionsRepository = movieSessionsRepository;
		_cacheHelper = cacheHelper;
		_mapper = mapper;
	}

	public override async Task<GetAllTicketsReply> GetAll(GetPaginatedDataRequest request, ServerCallContext context)
	{
		var key = $"{CacheConstants.TicketsPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";
		var tickets = _cacheHelper.Get<IList<Ticket>>(key);

		if (tickets is null)
		{
			tickets = await _ticketsRepository.GetAllAsync(request.PageNumber.Value, request.PageSize.Value);
			_cacheHelper.Set(key, tickets);
		}

		return _mapper.Map<GetAllTicketsReply>(tickets);
	}

	public override async Task<GetTicketByIdReply> GetById(GetTicketByIdRequest request, ServerCallContext context)
	{
		var key = $"{CacheConstants.TicketsPrefix}:{request.Id}";
		var ticket = _cacheHelper.Get<Ticket>(key);

		if (ticket is null)
		{
			ticket = await GetByIdOrThrowAsync(request.Id);
			_cacheHelper.Set(key, ticket);
		}

		return _mapper.Map<GetTicketByIdReply>(ticket);
	}

	public override async Task<GetAllTicketsReply> GetByUserId(GetTicketsByUserIdRequest request, ServerCallContext context)
	{
		var key = $"{CacheConstants.TicketsPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}:{request.UserId}";
		var tickets = _cacheHelper.Get<IList<Ticket>>(key);

		if (tickets is null)
		{
			tickets = await _ticketsRepository.GetByUserIdAsync(request.PageNumber.Value, request.PageSize.Value, request.UserId);
			_cacheHelper.Set(key, tickets);
		}

		return _mapper.Map<GetAllTicketsReply>(tickets);
	}

	public override async Task<CreateTicketReply> Create(CreateTicketRequest request, ServerCallContext context)
	{
		var movieSession = await GetByIdOrThrowAsync(request.MovieSessionId);

		var ticket = _mapper.Map<Ticket>(request);
		ticket.DateTime = movieSession.DateTime;
		ticket.Id = Guid.NewGuid();

		await _ticketsRepository.InsertAsync(ticket);

		return _mapper.Map<CreateTicketReply>(ticket);
	}

	private async Task<Ticket> GetByIdOrThrowAsync(string id)
	{
		var entity = await _ticketsRepository.GetByIdAsync(id)
			?? throw new NullReferenceException($"Ticket with id '{id}' does not exist.");

		return entity;
	}
}
