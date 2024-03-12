using Microsoft.OpenApi.Models;

namespace MovieTicketBookingApi.Configurations;

public static class SwaggerConfiguration
{
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddGrpcSwagger();
        services.AddSwaggerGen(options =>
        {
            const string ApiVersion = "v1";

            var assemblyName = typeof(Program).Assembly.GetName().Name;
            var xmlFilePath = Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.xml");

            options.SwaggerDoc(ApiVersion, new OpenApiInfo
            {
                Title = assemblyName,
                Version = ApiVersion
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
