namespace Core.Shared.Constants;

public static class AuthenticationOptionsConstants
{
    private const string Prefix = "Authentication";

    public const string Issuer = $"{Prefix}:Issuer";
    public const string Audience = $"{Prefix}:Audience";
    public const string Secret = $"{Prefix}:Secret";
}
