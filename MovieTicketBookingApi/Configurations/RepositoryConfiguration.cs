using Core.Interfaces;
using DataAccess.Repositories;

namespace MovieTicketBookingApi.Configurations;

public static class RepositoryConfiguration
{
    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IMoviesRepository, MoviesRepository>();
        services.AddScoped<IMovieHallsRepository, MovieHallsRepository>();
        services.AddScoped<IMovieSessionsRepository, MovieSessionsRepository>();
        services.AddScoped<ITicketsRepository, TicketsRepository>();
    }
}
