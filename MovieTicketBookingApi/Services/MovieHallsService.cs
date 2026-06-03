using Domain.Interfaces.Helpers;
using Domain.Interfaces.Repositories;
using Domain.Shared.Constants;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using MovieTicketBookingApi.Mappings;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.MovieHalls;
using MovieHall = Domain.Entities.MovieHall;

namespace MovieTicketBookingApi.Services;

[Authorize]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class MovieHallsService : MovieHalls.MovieHallsBase
{
	private readonly IMovieHallsRepository _repository;
	private readonly ICacheHelper _cacheHelper;

	public MovieHallsService(
		IMovieHallsRepository repository,
		ICacheHelper cacheHelper)
	{
		_repository = repository;
		_cacheHelper = cacheHelper;
	}

	[AllowAnonymous]
	public override async Task<GetAllMovieHallsReply> GetAll(
		GetPaginatedDataRequest request,
		ServerCallContext context)
	{
		var key = $"{CacheConstants.MovieHallsPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";
		var movieHalls = _cacheHelper.Get<IList<MovieHall>>(key);

		if (movieHalls is null)
		{
			movieHalls = await _repository.GetAllAsync(request.PageNumber.Value, request.PageSize.Value);
			_cacheHelper.Set(key, movieHalls);
		}

		return movieHalls.ToReply();
	}

	[AllowAnonymous]
	public override async Task<GetMovieHallByIdReply> GetById(
		GetMovieHallByIdRequest request,
		ServerCallContext context)
	{
		var key = $"{CacheConstants.MovieHallsPrefix}:{request.Id}";
		var movieHall = _cacheHelper.Get<MovieHall>(key);

		if (movieHall is null)
		{
			movieHall = await GetByIdOrThrowAsync(request.Id);
			_cacheHelper.Set(key, movieHall);
		}

		return movieHall.ToGetByIdReply();
	}

	public override async Task<CreateMovieHallReply> Create(
		CreateMovieHallRequest request,
		ServerCallContext context)
	{
		var movieHall = request.ToMovieHall();

		movieHall.Id = Guid.NewGuid();
		await _repository.InsertAsync(movieHall);

		return movieHall.ToCreateReply();
	}

	public override async Task<EmptyReply> Update(
		UpdateMovieHallRequest request,
		ServerCallContext context)
	{
		var movieHall = await _repository.GetByIdAsync(request.Id)
			?? throw new NullReferenceException($"Movie hall with id '{request.Id}' does not exist.");

		request.Update(movieHall);
		await _repository.UpdateAsync(movieHall);

		return new EmptyReply();
	}

	public override async Task<EmptyReply> Delete(
		DeleteMovieHallRequest request,
		ServerCallContext context)
	{
		await GetByIdOrThrowAsync(request.Id);
		await _repository.DeleteAsync(request.Id);

		return new EmptyReply();
	}

	private async Task<MovieHall> GetByIdOrThrowAsync(string id)
	{
		var entity = await _repository.GetByIdAsync(id)
			?? throw new NullReferenceException($"Movie hall with id '{id}' does not exist.");

		return entity;
	}
}
