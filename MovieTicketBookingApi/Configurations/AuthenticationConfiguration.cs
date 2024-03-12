using Core.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MovieTicketBookingApi.Configurations;

public static class AuthenticationConfiguration
{
	public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		var jwtOptionsSection = configuration.GetSection(JwtOptions.SectionName);
		var jwtOptions = jwtOptionsSection.Get<JwtOptions>()!;

		services.Configure<JwtOptions>(jwtOptionsSection);

		services
			.AddAuthorization()
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateActor = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ClockSkew = TimeSpan.Zero,
					ValidIssuer = jwtOptions.Issuer,
					ValidAudience = jwtOptions.Issuer,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
				};
			});
	}
}
