using System.Net;
using System.Net.Http.Json;
using BookingRooms.API.Common.Results;
using BookingRooms.API.Features.Auth.Register;
using FluentAssertions;

namespace BookingRooms.Specs.Integration.Registration;

[Collection("IntegrationTests")]
public class RegistrationTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
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
    
    [Fact]
    public async Task CreateUser_AsAdmin_ShouldReturnCreated()
    {
        // Arrange
        AsAdmin();
        
        var request = new RegisterUserRequest(
            UserName: "test@email.com",
            Password: "User123!",
            Role: "User");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task CreateUser_AsNormalUser_ShouldReturnForbidden()
    {
        // Arrange
        AsUser("normal-user-1");

        var request = new RegisterUserRequest(
            UserName: "test@test.com",
            Password: "User123!",
            Role: "User");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

}