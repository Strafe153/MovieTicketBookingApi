using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class MovieHallConfiguration : IEntityTypeConfiguration<MovieHall>
{
    public void Configure(EntityTypeBuilder<MovieHall> builder)
    {
        builder.HasKey(mh => mh.Id);

        builder
            .Property(mh => mh.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(mh => mh.NumberOfSeats)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .HasMany(mh => mh.MovieSessions)
            .WithOne(ms => ms.MovieHall);
    }
}
