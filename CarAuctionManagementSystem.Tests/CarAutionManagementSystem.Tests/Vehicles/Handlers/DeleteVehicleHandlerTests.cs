using CarAuctionManagementSystem.Handlers;
using CarAuctionManagementSystem.Services.Vehicles;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace CarAutionManagementSystem.Tests.Vehicles.Handlers
{
    public class DeleteVehicleHandlerTests
    {
        private readonly Mock<IVehicleService> _vehicleServiceMock;

        public DeleteVehicleHandlerTests()
        {
            _vehicleServiceMock = new Mock<IVehicleService>();
        }

        [Fact]
        public async Task DeleteVehicle_WhenCalled_ReturnsNoContent()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();

            _vehicleServiceMock
                .Setup(s => s.DeleteVehicleAsync(vehicleId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await VehicleHandlers.DeleteVehicle(vehicleId, _vehicleServiceMock.Object);

            // Assert
            result.Should().BeOfType<NoContent>();
        }
    }
}
