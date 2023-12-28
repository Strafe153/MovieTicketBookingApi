using Core.Entities;
using Core.Exceptions;
using Core.Interfaces.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace Core.Helpers;

public class PasswordHelper : IPasswordHelper
{
    public (byte[], byte[]) GeneratePasswordHashAndSalt(string password)
    {
        using var hmac = new HMACSHA256();
        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        var passwordSalt = hmac.Key;

        return (passwordHash, passwordSalt);
    }

    public void VerifyPasswordHash(string password, User user)
    {
        using var hmac = new HMACSHA256(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        if (!computedHash.SequenceEqual(user.PasswordHash))
        {
            throw new IncorrectPasswordException("Incorrect password.");
        }
    }
}
