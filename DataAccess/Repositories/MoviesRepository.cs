using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class MoviesRepository : IMoviesRepository
{
    private readonly MovieTicketBookingContext _context;

    public MoviesRepository(MovieTicketBookingContext context)
    {
        _context = context;
    }

    public Movie Create(Movie entity) =>
        _context.Movies.Add(entity).Entity;

    public void Delete(Movie entity) =>
        _context.Movies.Remove(entity);

    public async Task<IList<Movie>> GetAllAsync() =>
        await _context.Movies
            .Include(m => m.MovieSessions)
            .ToListAsync();

    public async Task<Movie?> GetByIdAsync(Guid id) =>
        await _context.Movies
            .Include(m => m.MovieSessions)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public void Update(Movie entity) =>
        _context.Movies.Update(entity);
}
