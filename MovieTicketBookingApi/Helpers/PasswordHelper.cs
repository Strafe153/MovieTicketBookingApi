using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace MovieTicketBookingApi.Helpers;

public class PasswordHelper : IPasswordHelper
{
	public (byte[], byte[]) GeneratePasswordHashAndSalt(string password)
	{
		using HMACSHA256 hmac = new();

		var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
		var passwordSalt = hmac.Key;

		return (passwordHash, passwordSalt);
	}

	public void VerifyPasswordHash(string password, User user)
	{
		using HMACSHA256 hmac = new(user.PasswordSalt);
		var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

		if (!computedHash.SequenceEqual(user.PasswordHash))
		{
			throw new IncorrectPasswordException("Incorrect password.");
		}
	}
}
