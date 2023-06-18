using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class MovieSessionConfiguration : IEntityTypeConfiguration<MovieSession>
{
    public void Configure(EntityTypeBuilder<MovieSession> builder)
    {
        builder.HasKey(ms => ms.Id);

        builder
            .Property(ms => ms.DateTime)
            .IsRequired();

        builder
            .HasMany(ms => ms.Tickets)
            .WithOne(t => t.MovieSession);

        builder
            .HasOne(ms => ms.Movie)
            .WithMany(m => m.MovieSessions);

        builder
            .HasOne(ms => ms.MovieHall)
            .WithMany(mh => mh.MovieSessions);
    }
}
