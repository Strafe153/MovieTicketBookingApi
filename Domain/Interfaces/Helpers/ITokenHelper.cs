using Domain.Entities;

namespace Domain.Interfaces.Helpers;

public interface ITokenHelper
{
	string GenerateAccessToken(User user);
}
