namespace Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;

    public IList<Ticket> Tickets { get; set; } = default!;
}
