using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingRooms.API.Features.Equipments;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipments");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);

        builder.HasData(
            new Equipment {Id = 1, Name = "Projector"},
            new Equipment {Id = 2, Name = "Whiteboard"},
            new Equipment {Id = 3, Name = "Conference Phone"},
            new Equipment {Id = 4, Name = "TV"}
        );
    }
}