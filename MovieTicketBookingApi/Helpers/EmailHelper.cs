using Domain.Interfaces.Helpers;
using FluentEmail.Core;

namespace MovieTicketBookingApi.Helpers;

public class EmailHelper : IEmailHelper
{
	private readonly IFluentEmail _fluentEmail;

	public EmailHelper(IFluentEmail fluentEmail)
	{
		_fluentEmail = fluentEmail;
	}

	public Task SendEmailAsync<T>(string email, string subject, string templateFileName, T model)
	{
		var parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory());
        var templateFilePath = $"{parentDirectory}/Domain/Email/Templates/{templateFileName}.cshtml";

		return _fluentEmail
            .To(email)
            .Subject(subject)
            .UsingTemplateFromFile(templateFilePath, model)
            .SendAsync();
    }
}
