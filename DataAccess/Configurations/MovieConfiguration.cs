using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(m => m.Id);

        builder
            .Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .HasMany(m => m.MovieSessions)
            .WithOne(ms => ms.Movie);
    }
}
