using Core.Entities;
using Core.Interfaces.Helpers;
using Core.Shared.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.Helpers;

public class TokenHelper : ITokenHelper
{
    private readonly IConfiguration _configuration;

    public TokenHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(nameof(user.Id), user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection(AuthenticationOptionsConstants.Secret).Value));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            issuer: _configuration.GetSection(AuthenticationOptionsConstants.Issuer).Value,
            audience: _configuration.GetSection(AuthenticationOptionsConstants.Audience).Value,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            notBefore: DateTime.UtcNow,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    //public string GenerateRefreshToken()
    //{
    //    var bytesForToken = RandomNumberGenerator.GetBytes(64);
    //    var refreshToken = Convert.ToBase64String(bytesForToken);

    //    return refreshToken;
    //}

    //public void SetRefreshToken(string refreshToken, User user, HttpResponse response)
    //{
    //    var expiryDate = DateTime.UtcNow.AddDays(7);
    //    var cookieOptions = new CookieOptions()
    //    {
    //        HttpOnly = true,
    //        Expires = expiryDate
    //    };

    //    user.RefreshToken = refreshToken;
    //    user.RefreshTokenExpiryDate = expiryDate;

    //    response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    //}

    //public void ValidateRefreshToken(Player player, string refreshToken)
    //{
    //    if (player.RefreshToken != refreshToken)
    //    {
    //        _logger.LogWarning("Player '{Name}' failed to validate a refresh token due to providing an incorrect one", player.Name);
    //        throw new InvalidTokenException("Token is not valid");
    //    }

    //    if (player.RefreshTokenExpiryDate < DateTime.UtcNow)
    //    {
    //        _logger.LogWarning("Player '{Name}' failed to validate a refresh token due to token expiration", player.Name);
    //        throw new InvalidTokenException("Token has expired");
    //    }

    //    _logger.LogInformation("Player '{Name}' successfully validated a refresh token", player.Name);
    //}
}
