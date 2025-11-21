using BookingRooms.API.Common.Results;

namespace BookingRooms.API.Features.Reservations;

public interface IReservationService
{
    Task<Result<GetReservationResponse>> CreateReservationAsync(CreateReservationRequest request, string userId);
    Task<Result<GetReservationResponse>> GetReservationAsync(int reservationId);
    Task<Result<List<GetReservationResponse>>> GetReservationsByUserAsync(string userId);
    Task<Result<List<GetReservationResponse>>> GetAllReservationsAsync();
    Task<Result> CancelReservationAsync(int reservationId);
}