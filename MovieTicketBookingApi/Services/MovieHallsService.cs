﻿using AutoMapper;
using Core.Extensions;
using Core.Interfaces.Helpers;
using Core.Interfaces.Repositories;
using Core.Shared;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.MovieHalls;
using MovieHall = Core.Entities.MovieHall;

namespace MovieTicketBookingApi.Services;

[Authorize]
[EnableRateLimiting("tokenBucket")]
public class MovieHallsService : MovieHalls.MovieHallsBase
{
    private readonly IMovieHallsRepository _repository;
    private readonly ICacheHelper _cacheHelper;
    private readonly IMapper _mapper;
    private readonly CacheOptions _cacheOptions;

    public MovieHallsService(
        IMovieHallsRepository repository,
        ICacheHelper cacheHelper,
        IMapper mapper,
        CacheOptions cacheOptions)
    {
        _repository = repository;
        _cacheHelper = cacheHelper;
        _mapper = mapper;
        _cacheOptions = cacheOptions;
    }

    [AllowAnonymous]
    public override async Task<GetAllMovieHallsReply> GetAll(GetPaginatedDataRequest request, ServerCallContext context)
    {
        var key = $"orders:{request.PageNumber ??= 1}:{request.PageSize ??= 5}";
        var movieHalls = _cacheHelper.Get<IList<MovieHall>>(key);

        if (movieHalls is null)
        {
            movieHalls = await _repository.GetAllAsync(request.PageNumber.Value, request.PageSize.Value);
            _cacheHelper.Set(key, movieHalls, _cacheOptions);
        }

        return _mapper.Map<GetAllMovieHallsReply>(movieHalls);
    }

    [AllowAnonymous]
    public override async Task<GetMovieHallByIdReply> GetById(GetMovieHallByIdRequest request, ServerCallContext context)
    {
        var key = $"orders:{request.Id}";
        var movieHall = _cacheHelper.Get<MovieHall>(key);

        if (movieHall is null)
        {
            movieHall = await _repository.GetByIdOrThrowAsync(request.Id);
            _cacheHelper.Set(key, movieHall, _cacheOptions);
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
