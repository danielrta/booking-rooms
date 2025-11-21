using BookingRooms.API.Features.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingRooms.API.Features.Reservations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.UserId)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(r => r.StartTimeUtc)
            .IsRequired();

        builder.Property(r => r.EndTimeUtc)
            .IsRequired();
        
        builder.Property(r => r.CreatedAtUtc)
            .IsRequired();
        
        builder.Property(r => r.Status)
            .HasConversion<int>()
            .IsRequired();
        
        builder.HasOne(r => r.Room)
            .WithMany()
            .HasForeignKey(r => r.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        
        builder.HasIndex(r=> new { r.RoomId, r.StartTimeUtc, r.EndTimeUtc })
            .IsUnique();
    }
}