using Core.Interfaces;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MovieTicketBookingApi.Extensions;
using MovieTicketBookingApi.Protos.Movies;

namespace MovieTicketBookingApi.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class MoviesService : Movies.MoviesBase
    {
        private readonly IMoviesRepository _moviesRepository;

        public MoviesService(IMoviesRepository moviesRepository)
        {
            _moviesRepository = moviesRepository;
        }

        public override async Task<GetMovieByIdReply> GetById(GetMovieByIdRequest request, ServerCallContext context)
        {
            var movie = await _moviesRepository.GetById(new Guid(request.Id));

            if (movie is null)
            {
                throw new NullReferenceException();
            }

            return new GetMovieByIdReply
            {
                Id = movie.Id.ToString(),
                Title = movie.Title,
                MovieSessions = { movie.MovieSessions.ToRepeatedField() }
            };
        }
    }
}
