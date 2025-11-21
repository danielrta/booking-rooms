using BookingRooms.API.Common.Results;
using BookingRooms.API.Data;
using BookingRooms.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BookingRooms.API.Features.Reservations;

public class ReservationService(BookingContext context, IPublicHolidayService publicHolidayService) : IReservationService
{
    public async Task<Result<GetReservationResponse>> CreateReservationAsync(CreateReservationRequest request, string userId)
    {
        var startLocal = request.Date.ToDateTime(request.StartTime);
        var endLocal   = request.Date.ToDateTime(request.EndTime);
    
        var startUtc = DateTime.SpecifyKind(startLocal, DateTimeKind.Local)
            .ToUniversalTime();

        var endUtc   = DateTime.SpecifyKind(endLocal, DateTimeKind.Local)
            .ToUniversalTime();
        
        var nowUtc = DateTime.UtcNow;
        
        if (startUtc <= nowUtc)
        {
            return Result<GetReservationResponse>.Failure(
                Error.Validation("Reservation start time must be in the future."));
        }
        
        if (startUtc > endUtc)
        {
            return Result<GetReservationResponse>.Failure(Error.Validation("Start time must be before end time."));
        }
        
        var roomExists = await context.Rooms.AnyAsync(r => r.Id == request.RoomId);
        
        if (!roomExists)
        {
            return Result<GetReservationResponse>.Failure(Error.NotFound("Room", request.RoomId));
        }
        
        var overlappingReservationExists = await context.Reservations.AnyAsync(r =>
            r.RoomId == request.RoomId &&
            r.Status == ReservationStatus.Active &&
            r.StartTimeUtc < endUtc &&
            r.EndTimeUtc   > startUtc);
        
        if (overlappingReservationExists)
        {
            return Result<GetReservationResponse>.Failure(Error.Conflict("The room is already booked for the requested time slot."));
        }

        var isPublicHoliday =
            await publicHolidayService.IsPublicPublicHolidayAsync(DateOnly.FromDateTime(startUtc), "MX");
        
        if (isPublicHoliday)
        {
            return Result<GetReservationResponse>.Failure(Error.Validation("Reservations cannot be made on public holidays."));
        }
        
        var newReservation = request.ToReservation(userId, startUtc, endUtc);
        
        context.Reservations.Add(newReservation);
        await context.SaveChangesAsync();
        
        await context.Entry(newReservation)
            .Reference(r => r.Room)
            .LoadAsync();
        
        await context.Entry(newReservation)
            .Reference(r => r.User)
            .LoadAsync();
        
        return Result<GetReservationResponse>.Success(newReservation.ToDto());
    }

    public Task<Result<GetReservationResponse>> GetReservationAsync(int reservationId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<GetReservationResponse>>> GetReservationsByUserAsync(string userId)
    {
        var reservation = await context.Reservations
            .AsNoTracking()
            .Include(r => r.Room)
            .Include(r => r.User)
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.StartTimeUtc)
            .ToListAsync();
        
        var response = reservation.Select(r => r.ToDto()).ToList();
        
        return Result<List<GetReservationResponse>>.Success(response);
    }

    public async Task<Result<List<GetReservationResponse>>> GetAllReservationsAsync()
    {
        var reservations = await context.Reservations
            .AsNoTracking()
            .Include(r => r.Room)
            .Include(r => r.User)
            .OrderBy(r => r.StartTimeUtc)
            .ToListAsync();
        
        var responses = reservations.Select(r => r.ToDto()).ToList();
        
        return Result<List<GetReservationResponse>>.Success(responses);
    }

    public async Task<Result> CancelReservationAsync(int reservationId)
    {
        var reservation = await context.Reservations.FindAsync(reservationId);
        
        if (reservation is null)
        {
            return Result.Failure(Error.NotFound("Reservation", reservationId));
        }
        
        if (reservation.Status == ReservationStatus.Cancelled)
        {
            return Result.Failure(Error.Validation("Reservation is already cancelled."));
        }
        
        reservation.Status = ReservationStatus.Cancelled;
        await context.SaveChangesAsync();
        
        return Result.Success();
    }
}