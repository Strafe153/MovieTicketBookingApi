using MovieTicketBookingApi.Configurations.ConfigurationModels;
using System.Net;
using System.Net.Mail;

namespace MovieTicketBookingApi.Configurations;

public static class FluentEmailConfiguration
{
	public static void ConfigureFluentEmail(this IServiceCollection services, IConfiguration configuration)
	{
		var options = configuration
			.GetSection(FluentEmailOptions.SectionName)
			.Get<FluentEmailOptions>()!;

		SmtpClient smtpClient = new()
		{
			Host = options.Host,
			Port = options.Port,
			Credentials = new NetworkCredential(options.Username, options.Password),
			EnableSsl = options.EnableSsl
		};

		services
			.AddFluentEmail(options.Sender)
			.AddSmtpSender(smtpClient)
			.AddRazorRenderer();
	}
}
