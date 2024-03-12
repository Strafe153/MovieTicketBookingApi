using Core.Helpers;
using Core.Interfaces.Helpers;
using Core.Shared.Constants;
using Microsoft.Extensions.Caching.Memory;

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
