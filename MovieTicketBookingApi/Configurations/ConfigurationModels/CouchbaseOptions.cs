namespace MovieTicketBookingApi.Configurations.ConfigurationModels;

public class CouchbaseOptions
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ConnectionString { get; set; } = default!;
    public bool HttpIgnoreRemoteCertificateMismatch { get; set; }
    public bool KvIgnoreRemoteCertificateNameMismatch { get; set; }
}
