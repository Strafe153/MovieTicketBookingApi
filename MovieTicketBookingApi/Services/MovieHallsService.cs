using AutoMapper;
using Core.Interfaces;
using Grpc.Core;
using MovieTicketBookingApi.Protos.Empty;
using MovieTicketBookingApi.Protos.MovieHalls;
using MovieTicketBookingApi.Protos.Paging;

namespace MovieTicketBookingApi.Services;

public class MovieHallsService : MovieHalls.MovieHallsBase
{
    private readonly IMovieHallsRepository _repository;
    private readonly IMapper _mapper;

    public MovieHallsService(
        IMovieHallsRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public override async Task<GetAllMovieHallsReply> GetAll(GetPaginatedDataRequest request, ServerCallContext context) =>
        _mapper.Map<GetAllMovieHallsReply>(await _repository.GetAllAsync());

    public override async Task<GetMovieHallByIdReply> GetById(GetMovieHallByIdRequest request, ServerCallContext context)
    {
        var movieHall = await _repository.GetByIdAsync(new Guid(request.Id));

        if (movieHall is null)
        {
            throw new NullReferenceException();
        }

        return _mapper.Map<GetMovieHallByIdReply>(movieHall);
    }

    public override async Task<CreateMovieHallReply> Create(CreateMovieHallRequest request, ServerCallContext context)
    {
        var movieHall = _repository.Create(_mapper.Map<Core.Entities.MovieHall>(request));
        await _repository.SaveChangesAsync();

        return _mapper.Map<CreateMovieHallReply>(movieHall);
    }

    public override async Task<EmptyReply> Update(UpdateMovieHallRequest request, ServerCallContext context)
    {
        var movieHall = await _repository.GetByIdAsync(new Guid(request.Id));

        if (movieHall is null)
        {
            throw new NullReferenceException();
        }

        _mapper.Map(request, movieHall);
        _repository.Update(movieHall);
        await _repository.SaveChangesAsync();

        return new EmptyReply();
    }

    public override async Task<EmptyReply> Delete(DeleteMovieHallRequest request, ServerCallContext context)
    {
        var movieHall = await _repository.GetByIdAsync(new Guid(request.Id));

        if (movieHall is null)
        {
            throw new NullReferenceException();
        }

        _repository.Delete(movieHall);
        await _repository.SaveChangesAsync();

        return new EmptyReply();
    }
}
