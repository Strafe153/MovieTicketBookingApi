using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DataAccess;

public class MovieTicketBookingContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<MovieSession> MovieSessions => Set<MovieSession>();
    public DbSet<MovieHall> MovieHalls => Set<MovieHall>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    public MovieTicketBookingContext(DbContextOptions<MovieTicketBookingContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
