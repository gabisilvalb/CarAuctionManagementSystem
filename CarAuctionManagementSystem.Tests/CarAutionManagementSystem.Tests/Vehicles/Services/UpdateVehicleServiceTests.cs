using CarAuctionManagement.Models.Enums;
using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Repositories.Auctions;
using CarAuctionManagementSystem.Repositories.Vehicles;
using CarAuctionManagementSystem.Services.Vehicles;
using FluentAssertions;
using Moq;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAutionManagementSystem.Tests.Vehicles.Services
{
    public class UpdateVehicleServiceTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
        private readonly VehicleService _vehicleService;

        public UpdateVehicleServiceTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _vehicleService = new VehicleService(_vehicleRepositoryMock.Object, _auctionRepositoryMock.Object);
        }

        [Fact]
        public async Task UpdateVehicleAsync_ValidUpdate_ReturnsUpdatedResponse()
        {
            // Arrange
            var id = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2020, 3000, 4);
            typeof(Vehicle).GetProperty("Id")!.SetValue(vehicle, id);

            var request = new UpdateVehicleRequest
            {
                Id = id,
                Manufacturer = "Mazda",
                Model = "3",
                Year = 2021,
                StartingBid = 3500,
                Type = VehicleType.Hatchback,
                NumberOfDoors = 5
            };

            _vehicleRepositoryMock.Setup(v => v.GetByIdAsync(id)).ReturnsAsync(vehicle);
            _auctionRepositoryMock.Setup(a => a.ExistsForVehicleAsync(id)).ReturnsAsync(false);
            _vehicleRepositoryMock.Setup(v => v.UpdateAsync(It.IsAny<Vehicle>())).ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleService.UpdateVehicleAsync(request);

            // Assert
            result.Should().NotBeNull();
            result!.Manufacturer.Should().Be("Mazda");
            result.Model.Should().Be("3");
            result.Year.Should().Be(2021);
            result.StartingBid.Should().Be(3500);
            result.NumberOfDoors.Should().Be(5);
        }

        [Fact]
        public async Task UpdateVehicleAsync_VehicleNotFound_ThrowsVehicleNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateVehicleRequest { Id = id, Type = VehicleType.SUV };

            _vehicleRepositoryMock.Setup(v => v.GetByIdAsync(id)).ReturnsAsync((Vehicle?)null);

            // Act & Assert
            var act = async () => await _vehicleService.UpdateVehicleAsync(request);
            await act.Should().ThrowAsync<VehicleNotFoundException>();
        }

        [Fact]
        public async Task UpdateVehicleAsync_ActiveAuction_ThrowsVehicleHaveActiveAuctionException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var vehicle = new SUV("Toyota", "RAV4", 2019, 4000, 5);
            typeof(Vehicle).GetProperty("Id")!.SetValue(vehicle, id);

            var request = new UpdateVehicleRequest { Id = id, Type = VehicleType.SUV };

            _vehicleRepositoryMock.Setup(v => v.GetByIdAsync(id)).ReturnsAsync(vehicle);
            _auctionRepositoryMock.Setup(a => a.ExistsForVehicleAsync(id)).ReturnsAsync(true);

            // Act & Assert
            var act = async () => await _vehicleService.UpdateVehicleAsync(request);
            await act.Should().ThrowAsync<VehicleHaveActiveAuctionException>();
        }

        [Fact]
        public async Task UpdateVehicleAsync_TypeChange_ThrowsCannotUpdateVehicleType()
        {
            // Arrange
            var id = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2020, 3000, 4);
            typeof(Vehicle).GetProperty("Id")!.SetValue(vehicle, id);

            var request = new UpdateVehicleRequest { Id = id, Type = VehicleType.SUV };

            _vehicleRepositoryMock.Setup(v => v.GetByIdAsync(id)).ReturnsAsync(vehicle);
            _auctionRepositoryMock.Setup(a => a.ExistsForVehicleAsync(id)).ReturnsAsync(false);

            // Act & Assert
            var act = async () => await _vehicleService.UpdateVehicleAsync(request);
            await act.Should().ThrowAsync<CannotUpdateVehicleType>();
        }

    }
}
