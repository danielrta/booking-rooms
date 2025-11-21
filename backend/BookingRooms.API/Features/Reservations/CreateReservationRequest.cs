namespace BookingRooms.API.Features.Reservations;

public sealed record CreateReservationRequest
(
    int RoomId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime
);