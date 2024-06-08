namespace MovieTicketBookingApi.Configurations.ConfigurationModels;

public class FluentEmailOptions
{
    public const string SectionName = "FluentEmail";

    public string Sender { get; set; } = default!;
    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool EnableSsl { get; set; }
}
