using MovieTicketBookingApi.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureGrpc();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureRepositories();
builder.Services.ConfigureAutoMapper();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.ApplySwagger();

app.MapGrpcServices();

app.Run();
