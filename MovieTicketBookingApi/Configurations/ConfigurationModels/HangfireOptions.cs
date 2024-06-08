namespace MovieTicketBookingApi.Configurations.ConfigurationModels;

public class HangfireOptions
{
    public const string SectionName = "Hangfire";

    public string User { get; set; } = default!;
    public string Password { get; set; } = default!;
}
