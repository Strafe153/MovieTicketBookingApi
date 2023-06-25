using Core.Entities;
using Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

    public async Task<IList<MovieSession>> GetAllAsync(int? pageNumber, int? pageSize)
    {
        var pageIndex = pageNumber.HasValue ? pageNumber.Value : 1;
        var pageLimit = pageSize.HasValue ? pageSize.Value : 5;

        return await _context.MovieSessions
            .Include(ms => ms.Tickets)
            .Skip((pageIndex - 1) * pageLimit)
            .Take(pageLimit)
            .ToListAsync();
    }

    public async Task<MovieSession?> GetByIdAsync(Guid id) =>
        await _context.MovieSessions
            .Include(ms => ms.Tickets)
            .FirstOrDefaultAsync(ms => ms.Id == id);

    public void Update(MovieSession entity) =>
        _context.MovieSessions.Update(entity);
}
