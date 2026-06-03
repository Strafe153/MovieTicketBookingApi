using Domain.Interfaces.Helpers;
using Domain.Interfaces.Repositories;
using Domain.Shared.Constants;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using MovieTicketBookingApi.Mappings;
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

	public MoviesService(
		IMoviesRepository repository,
		ICacheHelper cacheHelper)
	{
		_repository = repository;
		_cacheHelper = cacheHelper;
	}

	[AllowAnonymous]
	public override async Task<GetAllMoviesReply> GetAll(
		GetPaginatedDataRequest request,
		ServerCallContext context)
	{
		var key = $"{CacheConstants.MoviesPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";
		var movies = _cacheHelper.Get<IList<Movie>>(key);

		if (movies is null)
		{
			movies = await _repository.GetAllAsync(request.PageNumber.Value, request.PageSize.Value);
			_cacheHelper.Set(key, movies);
		}

		return movies.ToReply();
	}

	[AllowAnonymous]
	public override async Task<GetMovieByIdReply> GetById(
		GetMovieByIdRequest request,
		ServerCallContext context)
	{
		var key = $"{CacheConstants.MoviesPrefix}:{request.Id}";
		var movie = _cacheHelper.Get<Movie>(key);

		if (movie is null)
		{
			movie = await GetByIdOrThrowAsync(request.Id);
			_cacheHelper.Set(key, movie);
		}

		return movie.ToGetByIdReply();
	}

	public override async Task<CreateMovieReply> Create(
		CreateMovieRequest request,
		ServerCallContext context)
	{
		var movie = request.ToMovie();

		movie.Id = Guid.NewGuid();
		await _repository.InsertAsync(movie);

		return movie.ToCreateReply();
	}

	public override async Task<EmptyReply> Update(
		UpdateMovieRequest request,
		ServerCallContext context)
	{
		var movie = await GetByIdOrThrowAsync(request.Id);

		request.Update(movie);
		await _repository.UpdateAsync(movie);

		return new EmptyReply();
	}

	public override async Task<EmptyReply> Delete(
		DeleteMovieRequest request,
		ServerCallContext context)
	{
		await GetByIdOrThrowAsync(request.Id);
		await _repository.DeleteAsync(request.Id);

		return new EmptyReply();
	}

	private async Task<Movie> GetByIdOrThrowAsync(string id)
	{
		var entity = await _repository.GetByIdAsync(id)
			?? throw new NullReferenceException($"Movie with id '{id}' does not exist.");

		return entity;
	}
}
