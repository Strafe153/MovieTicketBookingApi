namespace Domain.Interfaces.Jobs;

public interface IAsyncJob
{
	Task ExecuteAsync(params object[] parameters);
}
