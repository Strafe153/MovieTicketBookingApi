using AutoMapper;
using Core.Extensions;
using Core.Interfaces.Helpers;
using Core.Interfaces.Repositories;
using Core.Shared.Constants;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.MovieSessions;
using MovieSession = Core.Entities.MovieSession;

namespace MovieTicketBookingApi.Services;

[Authorize]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class MovieSessionsService : MovieSessions.MovieSessionsBase
{
	private readonly IMovieSessionsRepository _repository;
	private readonly ICacheHelper _cacheHelper;
	private readonly IMapper _mapper;

	public MovieSessionsService(
		IMovieSessionsRepository repository,
		ICacheHelper cacheHelper,
		IMapper mapper)
	{
		_repository = repository;
		_cacheHelper = cacheHelper;
		_mapper = mapper;
	}

	[AllowAnonymous]
	public override async Task<GetAllMovieSessionsReply> GetAll(GetPaginatedDataRequest request, ServerCallContext context)
	{
		var key = $"{CacheConstants.MovieSessionsPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";
		var movieSessions = _cacheHelper.Get<IList<MovieSession>>(key);

		if (movieSessions is null)
		{
			movieSessions = await _repository.GetAllAsync(request.PageNumber.Value, request.PageSize.Value);
			_cacheHelper.Set(key, movieSessions);
		}

		return _mapper.Map<GetAllMovieSessionsReply>(movieSessions);
	}

	[AllowAnonymous]
	public override async Task<GetMovieSessionByIdReply> GetById(GetMovieSessionByIdRequest request, ServerCallContext context)
	{
		var key = $"{CacheConstants.MovieSessionsPrefix}:{request.Id}";
		var movieSession = _cacheHelper.Get<MovieSession>(key);

		if (movieSession is null)
		{
			movieSession = await _repository.GetByIdOrThrowAsync(request.Id);
			_cacheHelper.Set(key, movieSession);
		}

		return _mapper.Map<GetMovieSessionByIdReply>(movieSession);
	}

	public override async Task<CreateMovieSessionReply> Create(CreateMovieSessionRequest request, ServerCallContext context)
	{
		var movieSession = _mapper.Map<MovieSession>(request);
		movieSession.Id = Guid.NewGuid();

		await _repository.InsertAsync(movieSession);

		return _mapper.Map<CreateMovieSessionReply>(movieSession);
	}

	public override async Task<EmptyReply> Update(UpdateMovieSessionRequest request, ServerCallContext context)
	{
		var movieSession = await _repository.GetByIdOrThrowAsync(request.Id);

		_mapper.Map(request, movieSession);
		await _repository.UpdateAsync(movieSession);

		return new EmptyReply();
	}
}
