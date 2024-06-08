using Domain.Email.Models;

namespace Domain.Interfaces.Jobs;

public interface IRegistrationEmailJob
{
	Task ExecuteAsync(string email, UserRegistration userRegistration);
}
