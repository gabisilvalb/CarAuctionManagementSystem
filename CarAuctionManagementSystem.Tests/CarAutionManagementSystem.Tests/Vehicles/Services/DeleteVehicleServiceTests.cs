using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Repositories.Auctions;
using CarAuctionManagementSystem.Repositories.Vehicles;
using CarAuctionManagementSystem.Services.Vehicles;
using FluentAssertions;
using Moq;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAuctionManagementSystem.Tests.Vehicles.Services
{
    public class VehicleServiceTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepoMock;
        private readonly Mock<IAuctionRepository> _auctionRepoMock;
        private readonly VehicleService _vehicleService;

        public VehicleServiceTests()
        {
            _vehicleRepoMock = new Mock<IVehicleRepository>();
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _vehicleService = new VehicleService(_vehicleRepoMock.Object, _auctionRepoMock.Object);
        }

        [Fact]
        public async Task DeleteVehicleAsync_ValidId_DeletesVehicle()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);
            typeof(Vehicle).GetProperty("Id")!.SetValue(vehicle, vehicleId);

            _vehicleRepoMock
                .Setup(r => r.GetByIdAsync(vehicleId))
                .ReturnsAsync(vehicle);

            _auctionRepoMock
                .Setup(r => r.ExistsForVehicleAsync(vehicleId))
                .ReturnsAsync(false);

            _vehicleRepoMock
                .Setup(r => r.DeleteAsync(vehicleId))
                .Returns(Task.CompletedTask);

            // Act
            await _vehicleService.DeleteVehicleAsync(vehicleId);

            // Assert
            _vehicleRepoMock.Verify(r => r.DeleteAsync(vehicleId), Times.Once);
        }

        [Fact]
        public async Task DeleteVehicleAsync_VehicleNotFound_ThrowsVehicleNotFoundException()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();

            _vehicleRepoMock
                .Setup(r => r.GetByIdAsync(vehicleId))
                .ReturnsAsync((Vehicle?)null);

            // Act
            Func<Task> act = async () => await _vehicleService.DeleteVehicleAsync(vehicleId);

            // Assert
            await act.Should().ThrowAsync<VehicleNotFoundException>();
        }

        [Fact]
        public async Task DeleteVehicleAsync_VehicleHasActiveAuction_ThrowsVehicleHaveActiveAuctionException()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);
            typeof(Vehicle).GetProperty("Id")!.SetValue(vehicle, vehicleId);

            _vehicleRepoMock
                .Setup(r => r.GetByIdAsync(vehicleId))
                .ReturnsAsync(vehicle);

            _auctionRepoMock
                .Setup(r => r.ExistsForVehicleAsync(vehicleId))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _vehicleService.DeleteVehicleAsync(vehicleId);

            // Assert
            await act.Should().ThrowAsync<VehicleHaveActiveAuctionException>();
        }
    }
}
