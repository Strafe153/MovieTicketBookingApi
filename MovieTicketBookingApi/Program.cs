using Hangfire;
using MovieTicketBookingApi.Configurations;
using MovieTicketBookingApi.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHealthChecks();
builder.Services.ConfigureRateLimiting(builder.Configuration);

builder.Services.ConfigureRepositories();
builder.Services.ConfigureHelpers();
builder.Services.ConfigureAutoMapper();

builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureCouchbase(builder.Configuration);
builder.Services.ConfigureGrpc();
builder.Services.AddMemoryCache();

builder.Services.ConfigureHangfire();
builder.Services.ConfigureSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.ConfigureSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcServices();

app.MapGrpcHealthChecksService();

app.ConfigureCouchbaseLifetime();

app.UseHangfireDashboard();

await app.SetupDatabase();
RecurringJobsRegistry.RegisterRecurringJobs();

app.Run();
