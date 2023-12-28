using Microsoft.OpenApi.Models;

namespace MovieTicketBookingApi.Configurations;

public static class SwaggerConfiguration
{
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddGrpcSwagger();
        services.AddSwaggerGen(options =>
        {
            var xmlFilePath = Path.Combine(AppContext.BaseDirectory, "MovieTicketBookingApi.xml");

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "MovieTicketBookingApi",
                Version = "v1"
            });

            options.IncludeXmlComments(xmlFilePath);
            options.IncludeGrpcXmlComments(xmlFilePath, true);
        });
    }

    public static void ConfigureSwaggerUI(this WebApplication application)
    {
        application.UseSwagger();
        application.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie Ticket Booking Api");
        });
    }
}
