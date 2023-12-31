namespace MovieTicketBookingApi.Configurations.ConfigurationModels;

public class HangfireOptions
{
    public const string HangfireSectionName = "Hangfire";

    public string User { get; set; } = default!;
    public string Password { get; set; } = default!;
}
