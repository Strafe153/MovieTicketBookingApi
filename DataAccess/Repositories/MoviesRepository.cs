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

    public async Task<IList<Movie>> GetAll() =>
        await _context.Movies.ToListAsync();

    public async Task<Movie?> GetById(Guid id) =>
        await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
}
