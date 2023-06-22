using AutoMapper;
using Core.Interfaces;
using Grpc.Core;
using MovieTicketBookingApi.Protos.Empty;
using MovieTicketBookingApi.Protos.Movies;
using MovieTicketBookingApi.Protos.Paging;

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
        _mapper.Map<GetAllMoviesReply>(await _moviesRepository.GetAllAsync());

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
        var movie = _moviesRepository.Create(_mapper.Map<Core.Entities.Movie>(request));
        await _moviesRepository.SaveChangesAsync();

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
        _moviesRepository.Update(movie);
        await _moviesRepository.SaveChangesAsync();

        return new EmptyReply();
    }

    public override async Task<EmptyReply> Delete(DeleteMovieRequest request, ServerCallContext context)
    {
        var movie = await _moviesRepository.GetByIdAsync(new Guid(request.Id));

        if (movie is null)
        {
            throw new NullReferenceException();
        }

        _moviesRepository.Delete(movie);
        await _moviesRepository.SaveChangesAsync();

        return new EmptyReply();
    }
}
