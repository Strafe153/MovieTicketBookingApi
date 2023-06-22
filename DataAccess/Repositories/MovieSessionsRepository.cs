using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class MovieSessionsRepository : IMovieSessionsRepository
{
    private readonly MovieTicketBookingContext _context;

    public MovieSessionsRepository(MovieTicketBookingContext context)
    {
        _context = context;
    }

    public MovieSession Create(MovieSession entity) =>
        _context.MovieSessions.Add(entity).Entity;

    public void Delete(MovieSession entity) =>
        _context.MovieSessions.Remove(entity);

    public async Task<IList<MovieSession>> GetAllAsync() =>
        await _context.MovieSessions
            .Include(ms => ms.Tickets)
            .ToListAsync();

    public async Task<MovieSession?> GetByIdAsync(Guid id) =>
        await _context.MovieSessions
            .Include(ms => ms.Tickets)
            .FirstOrDefaultAsync(ms => ms.Id == id);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public void Update(MovieSession entity) =>
        _context.MovieSessions.Update(entity);
}
