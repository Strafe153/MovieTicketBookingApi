using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace MovieTicketBookingApi.Configurations;

public static class DbConfiguration
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MovieTicketBookingContext>(options =>
            options.UseOracle(configuration.GetConnectionString("DbConnection")));
    }
}
