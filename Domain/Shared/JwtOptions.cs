﻿namespace Domain.Shared;

public class JwtOptions
{
	public const string SectionName = "Jwt";

	public string Audience { get; set; } = default!;
	public string Issuer { get; set; } = default!;
	public string Secret { get; set; } = default!;
	public int ExpirationPeriod { get; set; } = 15;
}