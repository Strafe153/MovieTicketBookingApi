using Domain.Email.Models;
using Domain.Interfaces.Helpers;
using Domain.Interfaces.Jobs;

namespace MovieTicketBookingApi.Jobs.FireAndForgetJobs;

public class RegistrationEmailJob : IRegistrationEmailJob
{
	private const string Subject = "Registration";
	private const string TemplateFileName = "UserRegistration";

	private readonly IEmailHelper _emailHelper;

	public RegistrationEmailJob(IEmailHelper emailHelper)
	{
		_emailHelper = emailHelper;
	}

	public Task ExecuteAsync(string email, UserRegistration userRegistration) =>
		_emailHelper.SendEmailAsync(email, Subject, TemplateFileName, userRegistration);
}
