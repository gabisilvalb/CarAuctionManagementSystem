using CarAuctionManagement.Models.Auction;
using CarAuctionManagement.Models.Enums;
using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.Auction;
using CarAuctionManagementSystem.Repositories.Auctions;
using CarAuctionManagementSystem.Repositories.Bidders;
using CarAuctionManagementSystem.Repositories.Vehicles;
using CarAuctionManagementSystem.Services.Auctions;
using FluentAssertions;
using Moq;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAuctionManagementSystem.Tests.Auctions.Services
{
    public class StartAuctionServiceTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepoMock;
        private readonly Mock<IVehicleRepository> _vehicleRepoMock;
        private readonly Mock<IBidderRepository> _bidderRepoMock;
        private readonly AuctionService _auctionService;

        public StartAuctionServiceTests()
        {
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _vehicleRepoMock = new Mock<IVehicleRepository>();
            _bidderRepoMock = new Mock<IBidderRepository>();
            _auctionService = new AuctionService(_vehicleRepoMock.Object, _auctionRepoMock.Object, _bidderRepoMock.Object);
        }

        [Fact]
        public async Task StartAuctionAsync_ValidAuction_ReturnsStartAuctionResponse()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var vehicle = new Sedan("Toyota", "Yaris", 2022, 8000, 4);
            var auction = new Auction(vehicle, AuctionStatus.NotStarted, new List<Bid?>(), null, null);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            _auctionRepoMock
                .Setup(r => r.StartAuctionAsync(auction))
                .ReturnsAsync(() =>
                {
                    auction.Status = AuctionStatus.OnGoing;
                    return auction;
                });

            // Act
            var result = await _auctionService.StartAuctionAsync(auctionId);

            // Assert
            result.Should().NotBeNull();
            result.AuctionId.Should().Be(auctionId);
            result.Status.Should().Be(AuctionStatus.OnGoing);
            result.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task StartAuctionAsync_AuctionNotFound_ThrowsNoAuctionsFoundException()
        {
            // Arrange
            var auctionId = Guid.NewGuid();

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync((Auction?)null);

            // Act
            Func<Task> act = async () => await _auctionService.StartAuctionAsync(auctionId);

            // Assert
            await act.Should().ThrowAsync<NoAuctionsFoundException>();
        }

        [Fact]
        public async Task StartAuctionAsync_AuctionVehicleIsNull_ThrowsAuctionCantStartedException()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var auction = new Auction(null!, AuctionStatus.NotStarted, new List<Bid?>(), null, null);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            // Act
            Func<Task> act = async () => await _auctionService.StartAuctionAsync(auctionId);

            // Assert
            await act.Should().ThrowAsync<AuctionCantStartedException>();
        }

        [Fact]
        public async Task StartAuctionAsync_AuctionAlreadyStarted_ThrowsAuctionAlreadyStartedException()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);
            var auction = new Auction(vehicle, AuctionStatus.OnGoing, new List<Bid?>(), null, null);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            // Act
            Func<Task> act = async () => await _auctionService.StartAuctionAsync(auctionId);

            // Assert
            await act.Should().ThrowAsync<AuctionAlreadyStartedException>();
        }

        [Fact]
        public async Task StartAuctionAsync_AuctionAlreadyClosed_ThrowsAuctionAlreadyClosedException()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);
            var auction = new Auction(vehicle, AuctionStatus.Closed, new List<Bid?>(), null, null);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            // Act
            Func<Task> act = async () => await _auctionService.StartAuctionAsync(auctionId);

            // Assert
            await act.Should().ThrowAsync<AuctionAlreadyClosedException>();
        }
    }
}
