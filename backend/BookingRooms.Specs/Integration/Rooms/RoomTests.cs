using System.Net;
using System.Net.Http.Json;
using BookingRooms.API.Features.Auth;
using BookingRooms.API.Features.Rooms;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BookingRooms.Specs.Integration.Rooms;

[Collection("IntegrationTests")]
public class RoomTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
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
            name         = "Integration Room",
            capacity     = 10,
            location     = "1st Floor",
            equipmentIds = Array.Empty<int>()
        };

        var response = await _client.PostAsJsonAsync("/api/rooms", request);
        response.EnsureSuccessStatusCode();

        var room = await response.Content.ReadFromJsonAsync<GetRoomResponse>();
        room.Should().NotBeNull();

        return room!.Id;
    }
    
    [Fact]
    public async Task GetById_WhenRoomDoesNotExist_ReturnsNotFound()
    {
        AsAdmin();
        
        // Act
        var response = await _client.GetAsync("/api/rooms/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CreateRoom_Then_GetById_ReturnsCreatedRoom()
    {
        AsAdmin();
        
        // Arrange
        var createRequest = new
        {
            name = "Integration Room",
            capacity = 10,
            location = "First Floor",
            equipmentIds = new[] { 1, 2 }
        };

        // Act
        var createResponse = await _client.PostAsJsonAsync("/api/rooms", createRequest);

        // Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdRoom = await createResponse.Content.ReadFromJsonAsync<GetRoomResponse>();
        createdRoom.Should().NotBeNull();

        var createdId = createdRoom!.Id;
        
        var getResponse = await _client.GetAsync($"/api/rooms/{createdId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetchedRoom = await getResponse.Content.ReadFromJsonAsync<GetRoomResponse>();

        // Assert
        fetchedRoom.Should().NotBeNull();
        fetchedRoom!.Id.Should().Be(createdId);
        fetchedRoom.Name.Should().Be(createRequest.name);
        fetchedRoom.Capacity.Should().Be(createRequest.capacity);
        fetchedRoom.Location.Should().Be(createRequest.location);
        fetchedRoom.Equipments.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task UpdateRoom_ExistingRoom_UpdatesDataSuccessfully()
    {
        AsAdmin();
        
        // Arrange : Creating a new room
        var createRequest = new
        {
            name = "Room To Update",
            capacity = 8,
            location = "First Floor",
            equipmentIds = new[] { 1 }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/rooms", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var createdRoom = await createResponse.Content.ReadFromJsonAsync<GetRoomResponse>();
        createdRoom.Should().NotBeNull();
        var id = createdRoom!.Id;

        // Act: update the created room
        var updateRequest = new
        {
            name = "Room Updated",
            capacity = 12,
            location = "Second Floor",
            equipmentIds = new[] { 1, 2 }
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/rooms/{id}", updateRequest);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act: GET to verify updated data
        var getResponse = await _client.GetAsync($"/api/rooms/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedRoom = await getResponse.Content.ReadFromJsonAsync<GetRoomResponse>();
        updatedRoom.Should().NotBeNull();
        updatedRoom!.Name.Should().Be(updateRequest.name);
        updatedRoom.Capacity.Should().Be(updateRequest.capacity);
        updatedRoom.Location.Should().Be(updateRequest.location);
        updatedRoom.Equipments.Should().HaveCountGreaterThanOrEqualTo(2);
    }
    
    [Fact]
    public async Task UpdateRoom_NonExisting_ReturnsNotFound()
    {
       AsAdmin();
        
        // Arrange
        var updateRequest = new
        {
            name = "Non Existing Room",
            capacity = 5,
            location = "Nowhere",
            equipmentIds = new[] { 1 }
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/rooms/999999", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Room");
    }
    
    [Fact]
    public async Task DeleteRoom_WithoutReservations_ReturnsNoContent()
    {
        // Arrange: Create a room first
        AsAdmin();
        var roomId = await CreateRoomAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/rooms/{roomId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task DeleteRoom_WithFutureReservation_ReturnsConflict()
    {
        // Arrange: Create a room first
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

        var createReservationResponse =
            await _client.PostAsJsonAsync("/api/reservations", request);

        createReservationResponse.EnsureSuccessStatusCode();

        // Try to delete the room as admin
        AsAdmin();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/rooms/{roomId}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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