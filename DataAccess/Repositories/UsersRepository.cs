using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly MovieTicketBookingContext _context;

    public UsersRepository(MovieTicketBookingContext context)
    {
        _context = context;
    }

    public User Create(User entity) =>
        _context.Users.Add(entity).Entity;

    public void Delete(User entity) =>
        _context.Users.Remove(entity);

    public async Task<IList<User>> GetAllAsync(int? pageNumber, int? pageSize)
    {
        var pageIndex = pageNumber.HasValue ? pageNumber.Value : 1;
        var pageLimit = pageSize.HasValue ? pageSize.Value : 5;

        return await _context.Users
            .Include(u => u.Tickets)
            .Skip((pageIndex - 1) * pageLimit)
            .Take(pageLimit)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _context.Users
            .Include(u => u.Tickets)
            .FirstOrDefaultAsync(u => u.Id == id);

    public void Update(User entity) =>
        _context.Users.Update(entity);
}
