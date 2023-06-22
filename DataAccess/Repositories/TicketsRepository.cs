using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IList<Ticket>> GetAllAsync() =>
        await _context.Tickets.ToListAsync();

    public async Task<Ticket?> GetByIdAsync(Guid id) =>
        await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public void Update(Ticket entity) =>
        _context.Tickets.Update(entity);
}
