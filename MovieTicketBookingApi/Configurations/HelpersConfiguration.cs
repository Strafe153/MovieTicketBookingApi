using Domain.Interfaces.Helpers;
using Domain.Shared.Constants;
using Microsoft.Extensions.Caching.Memory;
using MovieTicketBookingApi.Helpers;

namespace MovieTicketBookingApi.Configurations;

public static class HelpersConfiguration
{
	public static void ConfigureHelpers(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSingleton<IPasswordHelper, PasswordHelper>();
		services.AddSingleton<ITokenHelper, TokenHelper>();
		services.AddSingleton<ICacheHelper, CacheHelper>();

		services.Configure<MemoryCacheEntryOptions>(configuration.GetSection(CacheConstants.SettingsSectionName));
	}
}
