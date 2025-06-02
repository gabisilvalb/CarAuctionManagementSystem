using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Repositories.Auctions;
using CarAuctionManagementSystem.Repositories.Vehicles;
using CarAuctionManagementSystem.Services.Vehicles;
using FluentAssertions;
using Moq;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAutionManagementSystem.Tests.Vehicles.Services
{
    public class GetVehiclesByIdServiceTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
        private readonly VehicleService _vehicleService;

        public GetVehiclesByIdServiceTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _vehicleService = new VehicleService(_vehicleRepositoryMock.Object, _auctionRepositoryMock.Object);
        }

        [Fact]
        public async Task GetVehicleByIdAsync_ValidId_ReturnsVehicle()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var expectedVehicle = new Hatchback("Toyota", "Yaris", 2022, 7000, 4);

            typeof(Vehicle).GetProperty("Id")!
                    .SetValue(expectedVehicle, vehicleId);

            _vehicleRepositoryMock
                .Setup(repo => repo.GetByIdAsync(vehicleId))
                .ReturnsAsync(expectedVehicle);

            // Act
            var result = await _vehicleService.GetVehicleByIdAsync(vehicleId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(vehicleId);
            result.Should().BeOfType<Hatchback>();
        }

        [Fact]
        public async Task GetVehicleByIdAsync_InvalidId_ThrowsNoVehiclesFoundException()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();

            _vehicleRepositoryMock
                .Setup(repo => repo.GetByIdAsync(vehicleId))
                .ReturnsAsync((Vehicle?)null);

            // Act
            var act = () => _vehicleService.GetVehicleByIdAsync(vehicleId);

            // Assert
            await act.Should().ThrowAsync<NoVehiclesFoundException>();
        }
    }
}
