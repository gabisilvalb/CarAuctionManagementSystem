using CarAuctionManagement.Models.Enums;
using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Handlers;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;
using CarAuctionManagementSystem.Services.Vehicles;
using FluentAssertions;
using Moq;

namespace CarAuctionManagementSystem.Tests.Vehicles.Handlers
{
    public class SearchVehiclesHandlerTests
    {
        private readonly Mock<IVehicleService> _vehicleServiceMock;

        public SearchVehiclesHandlerTests()
        {
            _vehicleServiceMock = new Mock<IVehicleService>();
        }

        [Fact]
        public async Task SearchVehicles_WithValidParams_ReturnsExpectedResults()
        {
            // Arrange
            var searchParams = new VehicleSearchParams
            {
                Manufacturer = "Ford",
                Type = VehicleType.Hatchback
            };

            var vehicles = new List<Vehicle>
                        {
                            new Hatchback("Ford", "Focus", 2021, 5000, 5),
                            new Hatchback("Ford", "Fiesta", 2020, 4500, 3)
                        };

            _vehicleServiceMock
                .Setup(s => s.SearchVehiclesAsync(searchParams))
                .ReturnsAsync(vehicles);

            // Act
            var result = await VehicleHandlers.SearchVehicles(searchParams, _vehicleServiceMock.Object);

            // Assert
            var responseObj = result.GetType().GetProperty("Value")?.GetValue(result);
            var vehicleList = responseObj?.GetType().GetProperty("result")?.GetValue(responseObj) as IEnumerable<VehicleResponse>;

            vehicleList.Should().NotBeNull();
            vehicleList!.Should().HaveCount(2);
            vehicleList.First().Manufacturer.Should().Be("Ford");
        }

    }
}
