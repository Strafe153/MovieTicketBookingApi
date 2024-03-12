using Core.Interfaces.Repositories;
using DataAccess.Repositories;

namespace MovieTicketBookingApi.Configurations;

public static class RepositoriesConfiguration
{
	public static void ConfigureRepositories(this IServiceCollection services) =>
		services
			.AddScoped<IUsersRepository, UsersRepository>()
			.AddScoped<IMoviesRepository, MoviesRepository>()
			.AddScoped<IMovieHallsRepository, MovieHallsRepository>()
			.AddScoped<IMovieSessionsRepository, MovieSessionsRepository>()
			.AddScoped<ITicketsRepository, TicketsRepository>();
}
