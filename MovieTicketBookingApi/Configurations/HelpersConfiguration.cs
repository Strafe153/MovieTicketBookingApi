using Core.Helpers;
using Core.Interfaces.Helpers;
using Core.Shared;

namespace MovieTicketBookingApi.Configurations;

public static class HelpersConfiguration
{
    public static void ConfigureHelpers(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHelper, PasswordHelper>();
        services.AddSingleton<ITokenHelper, TokenHelper>();
        services.AddSingleton<ICacheHelper, CacheHelper>();

        services.AddSingleton(new CacheOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
            SlidingExpiration = TimeSpan.FromSeconds(10)
        });
    }
}
