using Core.Entities;

namespace Core.Interfaces.Helpers;

public interface IPasswordHelper
{
    (byte[], byte[]) GeneratePasswordHashAndSalt(string password);
    void VerifyPasswordHash(string password, User user);
}
