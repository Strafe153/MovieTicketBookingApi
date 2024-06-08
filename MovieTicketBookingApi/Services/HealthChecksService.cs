using Domain.Shared.Constants;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MovieTicketBookingApi.Protos.Shared.Empty;
using MovieTicketBookingApi.Protos.V1.HealthChecks;

namespace MovieTicketBookingApi.Services;

[Authorize]
[EnableRateLimiting(RateLimitingConstants.TokenBucket)]
public class HealthChecksService : HealthCheck.HealthCheckBase
{
	private readonly HealthCheckService _healthCheckService;

	public HealthChecksService(HealthCheckService healthCheckService)
	{
		_healthCheckService = healthCheckService;
	}

	public override async Task<HealthCheckResponse> CheckHealth(EmptyRequest request, ServerCallContext context)
	{
		var result = await _healthCheckService.CheckHealthAsync(context.CancellationToken);

		var status = result.Status == HealthStatus.Healthy
			? ServingStatus.Serving
			: ServingStatus.NotServing;

		return new HealthCheckResponse
		{
			Status = status
		};
	}
}