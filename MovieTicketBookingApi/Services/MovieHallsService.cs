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
using MovieTicketBookingApi.Protos.V1.MovieHalls;
using MovieHall = Domain.Entities.MovieHall;

namespace MovieTicketBookingApi.Services;

[Authorize]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class MovieHallsService : MovieHalls.MovieHallsBase
{
	private readonly IMovieHallsRepository _repository;
	private readonly ICacheHelper _cacheHelper;
	private readonly IMapper _mapper;

	public MovieHallsService(
		IMovieHallsRepository repository,
		ICacheHelper cacheHelper,
		IMapper mapper)
	{
		_repository = repository;
		_cacheHelper = cacheHelper;
		_mapper = mapper;
	}

	[AllowAnonymous]
	public override async Task<GetAllMovieHallsReply> GetAll(GetPaginatedDataRequest request, ServerCallContext context)
	{
		var key = $"{CacheConstants.MovieHallsPrefix}:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";
		var movieHalls = _cacheHelper.Get<IList<MovieHall>>(key);

		if (movieHalls is null)
		{
			movieHalls = await _repository.GetAllAsync(request.PageNumber.Value, request.PageSize.Value);
			_cacheHelper.Set(key, movieHalls);
		}

		return _mapper.Map<GetAllMovieHallsReply>(movieHalls);
	}

	[AllowAnonymous]
	public override async Task<GetMovieHallByIdReply> GetById(GetMovieHallByIdRequest request, ServerCallContext context)
	{
		var key = $"{CacheConstants.MovieHallsPrefix}:{request.Id}";
		var movieHall = _cacheHelper.Get<MovieHall>(key);

		if (movieHall is null)
		{
			movieHall = await _repository.GetByIdOrThrowAsync(request.Id);
			_cacheHelper.Set(key, movieHall);
		}

		return _mapper.Map<GetMovieHallByIdReply>(movieHall);
	}

	public override async Task<CreateMovieHallReply> Create(CreateMovieHallRequest request, ServerCallContext context)
	{
		var movieHall = _mapper.Map<MovieHall>(request);
		movieHall.Id = Guid.NewGuid();

		await _repository.InsertAsync(movieHall);

		return _mapper.Map<CreateMovieHallReply>(movieHall);
	}

	public override async Task<EmptyReply> Update(UpdateMovieHallRequest request, ServerCallContext context)
	{
		var movieHall = await _repository.GetByIdOrThrowAsync(request.Id);

		_mapper.Map(request, movieHall);
		await _repository.UpdateAsync(movieHall);

		return new EmptyReply();
	}

	public override async Task<EmptyReply> Delete(DeleteMovieHallRequest request, ServerCallContext context)
	{
		await _repository.GetByIdOrThrowAsync(request.Id);
		await _repository.DeleteAsync(request.Id);

		return new EmptyReply();
	}
}
