using MovieTicketBookingApi.Services;

namespace MovieTicketBookingApi.Configurations;

public static class GrpcConfiguration
{
    public static void ConfigureGrpc(this IServiceCollection services)
    {
        services
            .AddGrpc()
            .AddJsonTranscoding();
    }

    public static void MapGrpcServices(this WebApplication application)
    {
        application.MapGrpcService<MoviesService>();
    }
}
