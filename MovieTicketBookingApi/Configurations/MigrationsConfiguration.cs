using DataAccess;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace MovieTicketBookingApi.Configurations;

public static class MigrationsConfiguration
{
    public static void ApplyDatabaseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MovieTicketBookingContext>();

        while (!dbContext.CanConnect())
        {
            Thread.Sleep(5000);
        }

        dbContext.Database.Migrate();
    }

    private static bool CanConnect(this MovieTicketBookingContext dbContext)
    {
        var connection = dbContext.Database.GetDbConnection();
        using var masterConnection = new OracleConnection(connection.ConnectionString);

        try
        {
            masterConnection.Open();
            masterConnection.Close();
        }
        catch (OracleException)
        {
            return false;
        }

        return true;
    }
}
