using BookingRooms.API.Features.Auth;
using BookingRooms.API.Features.Equipments;
using BookingRooms.API.Features.Reservations;
using BookingRooms.API.Features.Rooms;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingRooms.API.Data;

public class BookingContext(DbContextOptions<BookingContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingContext).Assembly);
    }
}