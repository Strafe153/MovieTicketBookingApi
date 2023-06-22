using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class MovieHallsRepository : IMovieHallsRepository
{
    private readonly MovieTicketBookingContext _context;

    public MovieHallsRepository(MovieTicketBookingContext context)
    {
        _context = context;
    }

    public MovieHall Create(MovieHall entity) =>
        _context.MovieHalls.Add(entity).Entity;

    public void Delete(MovieHall entity) =>
        _context.MovieHalls.Remove(entity);

    public async Task<IList<MovieHall>> GetAllAsync() =>
        await _context.MovieHalls
            .Include(mh => mh.MovieSessions)
            .ToListAsync();

    public async Task<MovieHall?> GetByIdAsync(Guid id) =>
        await _context.MovieHalls
            .Include(mh => mh.MovieSessions)
            .FirstOrDefaultAsync(mh => mh.Id == id);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public void Update(MovieHall entity) =>
        _context.MovieHalls.Update(entity);
}
