using Core.Interfaces.Repositories;
using DataAccess.Repositories;

namespace MovieTicketBookingApi.Configurations;

public static class RepositoriesConfiguration
{
    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IMoviesRepository, MoviesRepository>();
        services.AddScoped<IMovieHallsRepository, MovieHallsRepository>();
        services.AddScoped<IMovieSessionsRepository, MovieSessionsRepository>();
        services.AddScoped<ITicketsRepository, TicketsRepository>();
    }
}
