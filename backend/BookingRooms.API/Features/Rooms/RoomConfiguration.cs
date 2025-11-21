using BookingRooms.API.Features.Equipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingRooms.API.Features.Rooms;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(500);
        builder.Property(r => r.Capacity).IsRequired();
        builder.Property(r => r.Location).IsRequired().HasMaxLength(255);
        
        builder.HasMany(r => r.Equipments)
            .WithMany(e => e.Rooms)
            .UsingEntity<Dictionary<string, object>>(
                "RoomEquipment",
                re => re.HasOne<Equipment>().WithMany().HasForeignKey("EquipmentId"),
                re => re.HasOne<Room>().WithMany().HasForeignKey("RoomId"),
                re =>
                {
                    re.HasKey("RoomId", "EquipmentId");
                    re.ToTable("RoomEquipments");
                });
    }
}