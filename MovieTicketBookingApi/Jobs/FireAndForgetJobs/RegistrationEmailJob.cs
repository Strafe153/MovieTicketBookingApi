using Domain.Email.Models;
using Domain.Interfaces.Helpers;
using Domain.Shared.Constants;
using Quartz;

namespace MovieTicketBookingApi.Jobs.FireAndForgetJobs;

public class RegistrationEmailJob : IJob
{
    private const string Subject = "Registration";
    private const string TemplateFileName = "UserRegistration";

    private readonly IEmailHelper _emailHelper;

    public RegistrationEmailJob(IEmailHelper emailHelper)
    {
        _emailHelper = emailHelper;
    }

    public Task Execute(IJobExecutionContext context)
    {
        var email = context.MergedJobDataMap.GetString(JobConstants.RegistrationEmail.Email)!;
        var firstName = context.MergedJobDataMap.GetString(JobConstants.RegistrationEmail.FirstName)!;
        var lastName = context.MergedJobDataMap.GetString(JobConstants.RegistrationEmail.LastName)!;

        UserRegistration userRegistration = new(firstName, lastName);

        return _emailHelper.SendEmailAsync(email, Subject, TemplateFileName, userRegistration);
    }
}
