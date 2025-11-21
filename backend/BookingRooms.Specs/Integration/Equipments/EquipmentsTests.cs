using System.Net;
using System.Net.Http.Json;
using BookingRooms.API.Features.Equipments;
using FluentAssertions;

namespace BookingRooms.Specs.Integration;

[Collection("IntegrationTests")]
public class EquipmentsTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetAllEquipments_ShouldReturnOkAndListOfEquipments()
    {
        _client.DefaultRequestHeaders.Remove("X-Test-Role");
        _client.DefaultRequestHeaders.Add("X-Test-Role", "Admin");
        
        var response = await _client.GetAsync("/api/equipments");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var equipments = await response.Content.ReadFromJsonAsync<List<EquipmentResponse>>();
        equipments.Should().NotBeNull();
        equipments!.Count.Should().BeGreaterThan(0);
        
        equipments.Select(e=> e.Name).Should().Contain([
            "Projector",
            "Whiteboard",
        ]);
    }
}