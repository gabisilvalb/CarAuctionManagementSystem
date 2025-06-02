using CarAuctionManagement.Models.Auction;
using CarAuctionManagement.Models.Enums;
using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Repositories.Auctions;
using CarAuctionManagementSystem.Repositories.Bidders;
using CarAuctionManagementSystem.Repositories.Vehicles;
using CarAuctionManagementSystem.Services.Auctions;
using FluentAssertions;
using Moq;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAuctionManagementSystem.Tests.Auctions.Services
{
    public class AddAuctionServiceTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepoMock;
        private readonly Mock<IVehicleRepository> _vehicleRepoMock;
        private readonly Mock<IBidderRepository> _bidderRepoMock;
        private readonly AuctionService _auctionService;

        public AddAuctionServiceTests()
        {
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _vehicleRepoMock = new Mock<IVehicleRepository>();
            _bidderRepoMock = new Mock<IBidderRepository>();
            _auctionService = new AuctionService(_vehicleRepoMock.Object, _auctionRepoMock.Object, _bidderRepoMock.Object);
        }

        [Fact]
        public async Task AddAuctionAsync_ValidVehicle_ReturnsAuctionResponse()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);

            _vehicleRepoMock
                .Setup(r => r.GetByIdAsync(vehicleId))
                .ReturnsAsync(vehicle);

            _auctionRepoMock
                .Setup(r => r.ExistsForVehicleAsync(vehicleId))
                .ReturnsAsync(false);

            Auction? savedAuction = null;
            _auctionRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Auction>()))
                .Callback<Auction>(a => savedAuction = a)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _auctionService.AddAuctionAsync(vehicleId);

            // Assert
            result.Should().NotBeNull();
            result.Vehicle.Should().Be(vehicle);
            result.Status.Should().Be(AuctionStatus.NotStarted);
            savedAuction.Should().NotBeNull();
            savedAuction!.Vehicle.Should().Be(vehicle);
        }

        [Fact]
        public async Task AddAuctionAsync_InvalidVehicleId_ThrowsVehicleNotFoundException()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();

            _vehicleRepoMock
                .Setup(r => r.GetByIdAsync(vehicleId))
                .ReturnsAsync((Vehicle?)null);

            // Act
            Func<Task> act = async () => await _auctionService.AddAuctionAsync(vehicleId);

            // Assert
            await act.Should().ThrowAsync<VehicleNotFoundException>();
        }

        [Fact]
        public async Task AddAuctionAsync_VehicleAlreadyHasAuction_ThrowsAuctionSameIdException()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);

            _vehicleRepoMock
                .Setup(r => r.GetByIdAsync(vehicleId))
                .ReturnsAsync(vehicle);

            _auctionRepoMock
                .Setup(r => r.ExistsForVehicleAsync(vehicleId))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _auctionService.AddAuctionAsync(vehicleId);

            // Assert
            await act.Should().ThrowAsync<AuctionSameIdException>();
        }
    }
}
