using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    { 
        builder.HasKey(t => t.Id);

        builder
            .Property(t => t.SeatNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .HasOne(t => t.MovieSession)
            .WithMany(ms => ms.Tickets);

        builder
            .HasOne(t => t.User)
            .WithMany(u => u.Tickets);
    }
}
