using AutoMapper;
using Domain.Extensions;
using Domain.Interfaces.Helpers;
using Domain.Interfaces.Repositories;
using Domain.Shared.Constants;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.Movies;
using Movie = Domain.Entities.Movie;

namespace MovieTicketBookingApi.Services;

[Authorize]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class MoviesService : Movies.MoviesBase
{
	private readonly IMoviesRepository _repository;
	private readonly ICacheHelper _cacheHelper;
	private readonly IMapper _mapper;

	public MoviesService(
		IMoviesRepository repository,
		ICacheHelper cacheHelper,
		IMapper mapper)
	{
		_repository = repository;
		_cacheHelper = cacheHelper;
		_mapper = mapper;
	}

	[AllowAnonymous]
	public override async Task<GetAllMoviesReply> GetAll(GetPaginatedDataRequest request, ServerCallContext context)
	{
		var key = $"{CacheConstants.MoviesPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";
		var movies = _cacheHelper.Get<IList<Movie>>(key);

		if (movies is null)
		{
			movies = await _repository.GetAllAsync(request.PageNumber.Value, request.PageSize.Value);
			_cacheHelper.Set(key, movies);
		}

		return _mapper.Map<GetAllMoviesReply>(movies);
	}

	[AllowAnonymous]
	public override async Task<GetMovieByIdReply> GetById(GetMovieByIdRequest request, ServerCallContext context)
	{
		var key = $"{CacheConstants.MoviesPrefix}:{request.Id}";
		var movie = _cacheHelper.Get<Movie>(key);

		if (movie is null)
		{
			movie = await _repository.GetByIdOrThrowAsync(request.Id);
			_cacheHelper.Set(key, movie);
		}

		return _mapper.Map<GetMovieByIdReply>(movie);
	}

	public override async Task<CreateMovieReply> Create(CreateMovieRequest request, ServerCallContext context)
	{
		var movie = _mapper.Map<Movie>(request);
		movie.Id = Guid.NewGuid();

		await _repository.InsertAsync(movie);

		return _mapper.Map<CreateMovieReply>(movie);
	}

	public override async Task<EmptyReply> Update(UpdateMovieRequest request, ServerCallContext context)
	{
		var movie = await _repository.GetByIdOrThrowAsync(request.Id);

		_mapper.Map(request, movie);
		await _repository.UpdateAsync(movie);

		return new EmptyReply();
	}

	public override async Task<EmptyReply> Delete(DeleteMovieRequest request, ServerCallContext context)
	{
		await _repository.GetByIdOrThrowAsync(request.Id);
		await _repository.DeleteAsync(request.Id);

		return new EmptyReply();
	}
}
