using Domain.Interfaces.Helpers;
using Domain.Interfaces.Repositories;
using Domain.Shared.Constants;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using MovieTicketBookingApi.Mappings;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.Tickets;
using Ticket = Domain.Entities.Ticket;

namespace MovieTicketBookingApi.Services;

[Authorize]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class TicketsService : Tickets.TicketsBase
{
	private readonly ITicketsRepository _ticketsRepository;
	private readonly ICacheHelper _cacheHelper;

	public TicketsService(
		ITicketsRepository ticketsRepository,
		ICacheHelper cacheHelper)
	{
		_ticketsRepository = ticketsRepository;
		_cacheHelper = cacheHelper;
	}

	public override async Task<GetAllTicketsReply> GetAll(
		GetPaginatedDataRequest request,
		ServerCallContext context)
	{
		var key = $"{CacheConstants.TicketsPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";
		var tickets = _cacheHelper.Get<IList<Ticket>>(key);

		if (tickets is null)
		{
			tickets = await _ticketsRepository.GetAllAsync(request.PageNumber.Value, request.PageSize.Value);
			_cacheHelper.Set(key, tickets);
		}

		return tickets.ToReply();
	}

	public override async Task<GetTicketByIdReply> GetById(
		GetTicketByIdRequest request,
		ServerCallContext context)
	{
		var key = $"{CacheConstants.TicketsPrefix}:{request.Id}";
		var ticket = _cacheHelper.Get<Ticket>(key);

		if (ticket is null)
		{
			ticket = await GetByIdOrThrowAsync(request.Id);
			_cacheHelper.Set(key, ticket);
		}

		return ticket.ToGetByIdReply();
	}

	public override async Task<GetAllTicketsReply> GetByUserId(
		GetTicketsByUserIdRequest request,
		ServerCallContext context)
	{
		var key = $"{CacheConstants.TicketsPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}:{request.UserId}";
		var tickets = _cacheHelper.Get<IList<Ticket>>(key);

		if (tickets is null)
		{
			tickets = await _ticketsRepository.GetByUserIdAsync(request.PageNumber.Value, request.PageSize.Value, request.UserId);
			_cacheHelper.Set(key, tickets);
		}

		return tickets.ToReply();
	}

	public override async Task<CreateTicketReply> Create(
		CreateTicketRequest request,
		ServerCallContext context)
	{
		var ticket = request.ToTicket();
		ticket.Id = Guid.NewGuid();

		await _ticketsRepository.InsertAsync(ticket);

		return ticket.ToCreateReply();
	}

	private async Task<Ticket> GetByIdOrThrowAsync(string id)
	{
		var entity = await _ticketsRepository.GetByIdAsync(id)
			?? throw new NullReferenceException($"Ticket with id '{id}' does not exist.");

		return entity;
	}
}
