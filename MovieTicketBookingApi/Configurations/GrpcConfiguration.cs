using MovieTicketBookingApi.Interceptors;
using MovieTicketBookingApi.Services;

namespace MovieTicketBookingApi.Configurations;

public static class GrpcConfiguration
{
    public static void ConfigureGrpc(this IServiceCollection services) =>
        services
            .AddGrpc(options => options.Interceptors.Add<ExceptionHandlingInterceptor>())
            .AddJsonTranscoding();

    public static void MapGrpcServices(this WebApplication application)
    {
        application.MapGrpcService<UsersService>();
        application.MapGrpcService<MoviesService>();
        application.MapGrpcService<MovieHallsService>();
        application.MapGrpcService<MovieSessionsService>();
        application.MapGrpcService<TicketsService>();
        application.MapGrpcService<HealthChecksService>();
    }
}
