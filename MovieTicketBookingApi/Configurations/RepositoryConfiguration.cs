﻿using Core.Interfaces;
using DataAccess.Repositories;

namespace MovieTicketBookingApi.Configurations;

public static class RepositoryConfiguration
{
    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IMoviesRepository, MoviesRepository>();
    }
}