using Core.Shared.Constants;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace MovieTicketBookingApi.Configurations;

public static class RateLimitingConfiguration
{
    public static void ConfigureRateLimiting(this IServiceCollection services, IConfiguration configuration) =>
        services.AddRateLimiter(options =>
        {
            options.AddTokenBucketLimiter(RateLimitingConstants.TokenBucket, tokenOptions =>
            {
                var rateLimitOptions = configuration
                    .GetSection(RateLimitingConstants.SectionName)
                    .Get<TokenBucketRateLimiterOptions>()!;

                tokenOptions.TokenLimit = rateLimitOptions.TokenLimit;
                tokenOptions.TokensPerPeriod = rateLimitOptions.TokensPerPeriod;
                tokenOptions.AutoReplenishment = rateLimitOptions.AutoReplenishment;
                tokenOptions.ReplenishmentPeriod = rateLimitOptions.ReplenishmentPeriod;
                tokenOptions.QueueLimit = rateLimitOptions.QueueLimit;
                tokenOptions.QueueProcessingOrder = tokenOptions.QueueProcessingOrder;
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = (context, _) =>
            {
                context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
                return new ValueTask();
            };
        });
}
