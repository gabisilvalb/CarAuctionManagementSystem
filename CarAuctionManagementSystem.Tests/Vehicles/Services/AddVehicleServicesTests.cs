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
    public class AddVehicleServiceTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
        private readonly VehicleService _vehicleService;

        public AddVehicleServiceTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _vehicleService = new VehicleService(_vehicleRepositoryMock.Object, _auctionRepositoryMock.Object);
        }

        [Fact]
        public async Task AddVehicleAsync_WithHatchbackRequest_ReturnsAddedVehicle()
        {
            // Arrange
            var request = new AddVehicleRequest
            {
                Manufacturer = "Ford",
                Model = "Focus",
                Year = 2021,
                StartingBid = 5000m,
                Type = VehicleType.Hatchback,
                NumberOfDoors = 5
            };

            var expectedVehicle = new Hatchback(
                request.Manufacturer,
                request.Model,
                request.Year,
                request.StartingBid,
                request.NumberOfDoors.Value
            );

            _vehicleRepositoryMock
                .Setup(repo => repo.AddVehicleAsync(It.IsAny<Hatchback>()))
                .ReturnsAsync(expectedVehicle);

            // Act
            var result = await _vehicleService.AddVehicleAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Hatchback>();
            result.Manufacturer.Should().Be(request.Manufacturer);
            result.Model.Should().Be(request.Model);
            result.Year.Should().Be(request.Year);
            result.StartingBid.Should().Be(request.StartingBid);
            ((Hatchback)result).NumberOfDoors.Should().Be(request.NumberOfDoors);

            _vehicleRepositoryMock.Verify(repo => repo.AddVehicleAsync(It.IsAny<Hatchback>()), Times.Once);
        }
        [Fact]
        public async Task AddVehicleAsync_InvalidVehicleType_ShouldThrowInvalidVehicleTypeException()
        {
            // Arrange
            var request = new AddVehicleRequest
            {
                Manufacturer = "Invalid",
                Model = "Vehicle",
                Year = 2020,
                StartingBid = 2000,
                Type = (VehicleType)99
            };

            // Act
            var result = () => _vehicleService.AddVehicleAsync(request);

            // Assert
            await result.Should().ThrowAsync<InvalidVehicleTypeException>();
        }
    }
}
