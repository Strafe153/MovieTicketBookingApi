using AutoMapper;
using Core.Interfaces.Repositories;
using Grpc.Core;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.MovieSessions;

namespace MovieTicketBookingApi.Services;

public class MovieSessionsService : MovieSessions.MovieSessionsBase
{
    private readonly IMovieSessionsRepository _repository;
    private readonly IMapper _mapper;

    public MovieSessionsService(
        IMovieSessionsRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public override async Task<GetAllMovieSessionsReply> GetAll(GetPaginatedDataRequest request, ServerCallContext context) =>
        _mapper.Map<GetAllMovieSessionsReply>(await _repository.GetAllAsync(request.PageNumber, request.PageSize));

    public override async Task<GetMovieSessionByIdReply> GetById(GetMovieSessionByIdRequest request, ServerCallContext context)
    {
        var movieSession = await _repository.GetByIdAsync(new Guid(request.Id));

        if (movieSession is null)
        {
            throw new NullReferenceException();
        }

        return _mapper.Map<GetMovieSessionByIdReply>(movieSession);
    }

    public override async Task<CreateMovieSessionReply> Create(CreateMovieSessionRequest request, ServerCallContext context)
    {
        var movieSession = _mapper.Map<Core.Entities.MovieSession>(request);
        await _repository.Create(movieSession);

        return _mapper.Map<CreateMovieSessionReply>(movieSession);
    }

    public override async Task<EmptyReply> Update(UpdateMovieSessionRequest request, ServerCallContext context)
    {
        var movieSession = await _repository.GetByIdAsync(new Guid(request.Id));

        if (movieSession is null)
        {
            throw new NullReferenceException();
        }

        _mapper.Map(request, movieSession);
        await _repository.Update(movieSession);

        return new EmptyReply();
    }

    public override async Task<EmptyReply> Delete(DeleteMovieSessionRequest request, ServerCallContext context)
    {
        var movieHall = await _repository.GetByIdAsync(new Guid(request.Id));

        if (movieHall is null)
        {
            throw new NullReferenceException();
        }

        await _repository.Delete(movieHall.Id);

        return new EmptyReply();
    }
}
