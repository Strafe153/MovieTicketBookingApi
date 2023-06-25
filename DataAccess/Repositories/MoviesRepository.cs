using Core.Entities;
using Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

    public async Task<IList<Movie>> GetAllAsync(int? pageNumber, int? pageSize)
    {
        var pageIndex = pageNumber.HasValue ? pageNumber.Value : 1;
        var pageLimit = pageSize.HasValue ? pageSize.Value : 5;

        return await _context.Movies
            .Include(m => m.MovieSessions)
            .Skip((pageIndex - 1) * pageLimit)
            .Take(pageLimit)
            .ToListAsync(); ;
    }

    public async Task<Movie?> GetByIdAsync(Guid id) =>
        await _context.Movies
            .Include(m => m.MovieSessions)
            .FirstOrDefaultAsync(m => m.Id == id);

    public void Update(Movie entity) =>
        _context.Movies.Update(entity);
}
