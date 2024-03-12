namespace Domain.Interfaces.Helpers;

public interface ICacheHelper
{
	T? Get<T>(string key);
	void Set<T>(string key, T value);
}
