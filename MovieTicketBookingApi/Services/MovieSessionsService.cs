using Domain.Interfaces.Helpers;
using Domain.Interfaces.Repositories;
using Domain.Shared.Constants;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using MovieTicketBookingApi.Mappings;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.MovieSessions;
using MovieSession = Domain.Entities.MovieSession;

namespace MovieTicketBookingApi.Services;

[Authorize]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class MovieSessionsService : MovieSessions.MovieSessionsBase
{
	private readonly IMovieSessionsRepository _repository;
	private readonly ICacheHelper _cacheHelper;

	public MovieSessionsService(
		IMovieSessionsRepository repository,
		ICacheHelper cacheHelper)
	{
		_repository = repository;
		_cacheHelper = cacheHelper;
	}

	[AllowAnonymous]
	public override async Task<GetAllMovieSessionsReply> GetAll(
		GetPaginatedDataRequest request,
		ServerCallContext context)
	{
		var key = $"{CacheConstants.MovieSessionsPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";
		var movieSessions = _cacheHelper.Get<IList<MovieSession>>(key);

		if (movieSessions is null)
		{
			movieSessions = await _repository.GetAllAsync(request.PageNumber.Value, request.PageSize.Value);
			_cacheHelper.Set(key, movieSessions);
		}

		return movieSessions.ToReply();
	}

	[AllowAnonymous]
	public override async Task<GetMovieSessionByIdReply> GetById(
		GetMovieSessionByIdRequest request,
		ServerCallContext context)
	{
		var key = $"{CacheConstants.MovieSessionsPrefix}:{request.Id}";
		var movieSession = _cacheHelper.Get<MovieSession>(key);

		if (movieSession is null)
		{
			movieSession = await GetByIdOrThrowAsync(request.Id);
			_cacheHelper.Set(key, movieSession);
		}

		return movieSession.ToGetByIdReply();
	}

	public override async Task<CreateMovieSessionReply> Create(
		CreateMovieSessionRequest request,
		ServerCallContext context)
	{
		var movieSession = request.ToSession();

		movieSession.Id = Guid.NewGuid();
		await _repository.InsertAsync(movieSession);

		return movieSession.ToCreateReply();
	}

	public override async Task<EmptyReply> Update(
		UpdateMovieSessionRequest request,
		ServerCallContext context)
	{
		var movieSession = await GetByIdOrThrowAsync(request.Id);

		request.Update(movieSession);
		await _repository.UpdateAsync(movieSession);

		return new EmptyReply();
	}

	private async Task<MovieSession> GetByIdOrThrowAsync(string id)
	{
		var entity = await _repository.GetByIdAsync(id)
			?? throw new NullReferenceException($"Movie session with id '{id}' does not exist.");

		return entity;
	}
}
