using Core.Interfaces.Repositories;

namespace DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly MovieTicketBookingContext _context;

    public UnitOfWork(MovieTicketBookingContext context)
    {
        _context = context;   
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
