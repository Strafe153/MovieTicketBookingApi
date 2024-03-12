using MovieTicketBookingApi.AutoMapperProfiles;
using MovieTicketBookingApi.Configurations;
using MovieTicketBookingApi.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHealthChecks();
builder.Services.ConfigureRateLimiting(builder.Configuration);

builder.Services.ConfigureRepositories();
builder.Services.ConfigureHelpers(builder.Configuration);

builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureCouchbase(builder.Configuration);

builder.Services.ConfigureGrpc();
builder.Services.AddMemoryCache();
builder.Services.ConfigureHangfire();

builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);

builder.Services.ConfigureSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.ConfigureSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcServices();

app.MapGrpcHealthChecksService();

app.ConfigureCouchbaseLifetime();

app.UseAuthenticatedHangfireDashboard(builder.Configuration);

await app.SetupDatabase();
RecurringJobsRegistry.RegisterRecurringJobs();

app.Run();
