namespace Domain.Interfaces.Helpers;

public interface IEmailHelper
{
	Task SendEmailAsync<T>(string email, string subject, string templateFileName, T model);
}
