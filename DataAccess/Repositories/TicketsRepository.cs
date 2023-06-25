using Core.Entities;
using Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DataAccess.Repositories;

public class TicketsRepository : ITicketsRepository
{
    private readonly MovieTicketBookingContext _context;

    public TicketsRepository(MovieTicketBookingContext context)
    {
        _context = context;
    }

    public Ticket Create(Ticket entity) =>
        _context.Tickets.Add(entity).Entity;

    public void Delete(Ticket entity) =>
        _context.Tickets.Remove(entity);

    public async Task<IList<Ticket>> GetAllAsync(int? pageNumber, int? pageSize)
    {
        var pageIndex = pageNumber.HasValue ? pageNumber.Value : 1;
        var pageLimit = pageSize.HasValue ? pageSize.Value : 5;

        return await _context.Tickets
            .Skip((pageIndex - 1) * pageLimit)
            .Take(pageLimit)
            .ToListAsync();
    }

    public async Task<Ticket?> GetByIdAsync(Guid id) =>
        await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);

    public void Update(Ticket entity) =>
        _context.Tickets.Update(entity);
}
