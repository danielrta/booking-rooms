using System.Net;
using System.Net.Http.Json;
using BookingRooms.API.Features.Auth;
using BookingRooms.API.Features.Reservations;
using BookingRooms.API.Features.Rooms;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BookingRooms.Specs.Integration.Reservations;

[Collection("IntegrationTests")]
public class ReservationsTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    
    private void AsAdmin(string userId = "admin-id")
    {
        _client.DefaultRequestHeaders.Remove("X-Test-Role");
        _client.DefaultRequestHeaders.Remove("X-Test-UserId");
        _client.DefaultRequestHeaders.Remove("X-Test-UserName");

        _client.DefaultRequestHeaders.Add("X-Test-Role", "Admin");
        _client.DefaultRequestHeaders.Add("X-Test-UserId", userId);
        _client.DefaultRequestHeaders.Add("X-Test-UserName", "admin");
    }

    private void AsUser(string userId)
    {
        _client.DefaultRequestHeaders.Remove("X-Test-Role");
        _client.DefaultRequestHeaders.Remove("X-Test-UserId");
        _client.DefaultRequestHeaders.Remove("X-Test-UserName");

        _client.DefaultRequestHeaders.Add("X-Test-Role", "User");
        _client.DefaultRequestHeaders.Add("X-Test-UserId", userId);
        _client.DefaultRequestHeaders.Add("X-Test-UserName", userId);
    }
    
    private async Task<int> CreateRoomAsync()
    {
        var request = new
        {
            name        = "Integration Room",
            capacity    = 10,
            location    = "1st Floor",
            equipmentIds = Array.Empty<int>()
        };

        var response = await _client.PostAsJsonAsync("/api/rooms", request);
        response.EnsureSuccessStatusCode();

        var room = await response.Content.ReadFromJsonAsync<GetRoomResponse>();
        room.Should().NotBeNull();

        return room!.Id;
    }
    
    [Fact]
    public async Task CreateReservation_NoOverlap_ReturnsCreated()
    {
        // Arrange
        AsAdmin();
        var roomId = await CreateRoomAsync();
        
        const string userName = "user1@example.com";
        var userId   = await EnsureTestUserAsync(userName);
        
        AsUser(userId);
        
        var baseDateTime = DateTime.UtcNow.AddDays(1);

        var date      = DateOnly.FromDateTime(baseDateTime);
        var startTime = TimeOnly.FromDateTime(baseDateTime);
        var endTime   = startTime.AddHours(1);

        var request = new
        {
            roomId,
            date,
            startTime,
            endTime
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var reservation = await response.Content.ReadFromJsonAsync<GetReservationResponse>();
        reservation.Should().NotBeNull();
        reservation!.RoomId.Should().Be(roomId);
        reservation.UserId.Should().Be(userId);
    }
    
    [Fact]
    public async Task CreateReservation_OverlappingSameRoom_ReturnsConflict()
    {
        // Arrange
        AsAdmin();
        var roomId = await CreateRoomAsync();
    
        const string userName = "user1@example.com";
        var userId   = await EnsureTestUserAsync(userName);
        
        AsUser(userId);

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var firstRequest = new
        {
            roomId,
            date,
            startTime = new TimeOnly(10, 0, 0),
            endTime   = new TimeOnly(11, 0, 0)
        };

        var firstResponse = await _client.PostAsJsonAsync("/api/reservations", firstRequest);
        firstResponse.EnsureSuccessStatusCode();

        // Create overlapping reservation
        var overlappingRequest = new
        {
            roomId,
            date,
            startTime = new TimeOnly(10, 30, 0),
            endTime   = new TimeOnly(11, 30, 0)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", overlappingRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task GetReservations_AsAdmin_ReturnsAll()
    {
        // Arrange: Create two reservations as user
        AsAdmin();
        var roomId = await CreateRoomAsync();

        const string userName = "user1@example.com";
        var userId   = await EnsureTestUserAsync(userName);
        
        AsUser(userId);
        
        var baseDateTime = DateTime.UtcNow.AddDays(1);

        var date      = DateOnly.FromDateTime(baseDateTime);
        var startTime = TimeOnly.FromDateTime(baseDateTime);
        var endTime   = startTime.AddHours(1);
        
        var r1 = new
        {
            roomId,
            date,
            startTime = startTime,
            endTime   = endTime
        };
        var r2 = new
        {
            roomId,
            date,
            startTime = startTime.AddHours(3),
            endTime   = endTime.AddHours(3)
        };

        await _client.PostAsJsonAsync("/api/reservations", r1);
        await _client.PostAsJsonAsync("/api/reservations", r2);

        // Acting as admin to get all reservations
        AsAdmin();

        // Act
        var response = await _client.GetAsync("/api/reservations");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await response.Content.ReadFromJsonAsync<List<GetReservationResponse>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeGreaterThanOrEqualTo(2);
    }
    
    [Fact]
    public async Task GetReservations_AsUser_ReturnsOnlyHis()
    {
        AsAdmin();
        var roomId = await CreateRoomAsync();
        var today  = DateTime.UtcNow.Date;

        // Reservation for user-1
        AsUser("user-1");
        var rUser1 = new
        {
            roomId,
            startTimeUtc = today.AddHours(9),
            endTimeUtc   = today.AddHours(10)
        };
        await _client.PostAsJsonAsync("/api/reservations", rUser1);

        // Reservation for user-2
        AsUser("user-2");
        var rUser2 = new
        {
            roomId,
            startTimeUtc = today.AddHours(11),
            endTimeUtc   = today.AddHours(12)
        };
        await _client.PostAsJsonAsync("/api/reservations", rUser2);

        // Acting as user-1 to get his reservations
        AsUser("user-1");
        var response = await _client.GetAsync("/api/reservations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await response.Content.ReadFromJsonAsync<List<GetReservationResponse>>();
        list.Should().NotBeNull();
        list!.Should().OnlyContain(r => r.UserId == "user-1");
    }
    
    [Fact]
    public async Task CreateReservation_OnPublicHoliday_ReturnsConflict()
    {
        // Configure fake public holiday service to have a holiday on 2025-01-01
        using (var scope = factory.Services.CreateScope())
        {
            var fake = scope.ServiceProvider.GetRequiredService<FakePublicHolidayService>();
            fake.PublicHolidays.Add(new DateOnly(2025, 1, 1));
        }

        AsAdmin();
        var roomId = await CreateRoomAsync();

        AsUser("user-holiday");

        var date = new DateTime(2025, 1, 1);

        var request = new
        {
            roomId,
            date,
            startTime = new TimeOnly(10, 0, 0),
            endTime   = new TimeOnly(11, 0, 0)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/reservations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    private async Task<string> GetUserIdAsync(string userName)
    {
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByNameAsync(userName);
        return user is null ? throw new InvalidOperationException($"User '{userName}' not found in test DB.") : user.Id;
    }
    
    private async Task<string> EnsureTestUserAsync(string userName)
    {
        using var scope = factory.Services.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = userName,
                Email = userName
            };
            
            var result = await userManager.CreateAsync(user, "Password123!");

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create test user '{userName}': {errors}");
            }
        }

        return user.Id;
    }

}