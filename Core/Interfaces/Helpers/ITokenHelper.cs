using Core.Entities;

namespace Core.Interfaces.Helpers;

public interface ITokenHelper
{
    string GenerateAccessToken(User user);
}
