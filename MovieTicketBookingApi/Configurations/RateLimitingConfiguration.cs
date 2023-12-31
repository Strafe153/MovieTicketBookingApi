using Microsoft.AspNetCore.RateLimiting;
using MovieTicketBookingApi.Configurations.ConfigurationModels;

namespace MovieTicketBookingApi.Configurations;

public static class RateLimitingConfiguration
{
    public static void ConfigureRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var rateLimitOptions = configuration
            .GetSection(RateLimitOptions.RateLimitOptionsSectionName)
            .Get<RateLimitOptions>()!;

        services.AddRateLimiter(options =>
        {
            options.AddTokenBucketLimiter("tokenBucket", tokenOptions =>
            {
                tokenOptions.TokenLimit = rateLimitOptions.TokenLimit;
                tokenOptions.TokensPerPeriod = rateLimitOptions.TokensPerPeriod;
                tokenOptions.AutoReplenishment = rateLimitOptions.AutoReplenishment;
                tokenOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(rateLimitOptions.ReplenishmentPeriod);
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
}
