using Domain.Entities;
using Domain.Interfaces.Helpers;
using Domain.Shared;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieTicketBookingApi.Helpers;

public class TokenHelper : ITokenHelper
{
	private readonly JwtOptions _jwtOptions;

	public TokenHelper(IOptions<JwtOptions> jwtOptions)
	{
		_jwtOptions = jwtOptions.Value;
	}

	public string GenerateAccessToken(User user)
	{
		List<Claim> claims =
		[
			new(ClaimTypes.Email, user.Email),
			new(nameof(user.Id), user.Id.ToString())
		];

		SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
		SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256Signature);

		JwtSecurityToken token = new(
			issuer: _jwtOptions.Issuer,
			audience: _jwtOptions.Audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationPeriod),
			notBefore: DateTime.UtcNow,
			signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}
