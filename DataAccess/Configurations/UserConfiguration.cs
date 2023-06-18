using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder
            .Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .HasMany(u => u.Tickets)
            .WithOne(t => t.User);
    }
}
