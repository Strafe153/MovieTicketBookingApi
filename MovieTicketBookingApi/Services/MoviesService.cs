using AutoMapper;
using Core.Interfaces.Repositories;
using Grpc.Core;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.Shared.Paging;
using MovieTicketBookingApi.Protos.V1.Movies;

namespace MovieTicketBookingApi.Services;

public class MoviesService : Movies.MoviesBase
{
    private readonly IMoviesRepository _moviesRepository;
    private readonly IMapper _mapper;

    public MoviesService(
        IMoviesRepository moviesRepository,
        IMapper mapper)
    {
        _moviesRepository = moviesRepository;
        _mapper = mapper;
    }

    public override async Task<GetAllMoviesReply> GetAll(GetPaginatedDataRequest request, ServerCallContext context) =>
        _mapper.Map<GetAllMoviesReply>(await _moviesRepository.GetAllAsync(request.PageNumber, request.PageSize));

    public override async Task<GetMovieByIdReply> GetById(GetMovieByIdRequest request, ServerCallContext context)
    {
        var movie = await _moviesRepository.GetByIdAsync(new Guid(request.Id));

        if (movie is null)
        {
            throw new NullReferenceException();
        }

        return _mapper.Map<GetMovieByIdReply>(movie);
    }

    public override async Task<CreateMovieReply> Create(CreateMovieRequest request, ServerCallContext context)
    {
        var movie = _mapper.Map<Core.Entities.Movie>(request);
        await _moviesRepository.Create(movie);

        return _mapper.Map<CreateMovieReply>(movie);
    }

    public override async Task<EmptyReply> Update(UpdateMovieRequest request, ServerCallContext context)
    {
        var movie = await _moviesRepository.GetByIdAsync(new Guid(request.Id));

        if (movie is null)
        {
            throw new NullReferenceException();
        }

        _mapper.Map(request, movie);
        await _moviesRepository.Update(movie);

        return new EmptyReply();
    }

    public override async Task<EmptyReply> Delete(DeleteMovieRequest request, ServerCallContext context)
    {
        var movie = await _moviesRepository.GetByIdAsync(new Guid(request.Id));

        if (movie is null)
        {
            throw new NullReferenceException();
        }

        await _moviesRepository.Delete(movie.Id);

        return new EmptyReply();
    }
}
