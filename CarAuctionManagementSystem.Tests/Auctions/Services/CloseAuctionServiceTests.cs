using CarAuctionManagement.Models.Auction;
using CarAuctionManagement.Models.Enums;
using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.Auction;
using CarAuctionManagementSystem.Models.Bidder;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Repositories.Auctions;
using CarAuctionManagementSystem.Repositories.Bidders;
using CarAuctionManagementSystem.Repositories.Vehicles;
using CarAuctionManagementSystem.Services.Auctions;
using FluentAssertions;
using Moq;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAuctionManagementSystem.Tests.Auctions.Services
{
    public class CloseAuctionServiceTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepoMock;
        private readonly Mock<IVehicleRepository> _vehicleRepoMock;
        private readonly Mock<IBidderRepository> _bidderRepoMock;
        private readonly AuctionService _auctionService;

        public CloseAuctionServiceTests()
        {
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _vehicleRepoMock = new Mock<IVehicleRepository>();
            _bidderRepoMock = new Mock<IBidderRepository>();
            _auctionService = new AuctionService(_vehicleRepoMock.Object, _auctionRepoMock.Object, _bidderRepoMock.Object);
        }

        [Fact]
        public async Task CloseAuctionAsync_ValidRequest_ReturnsCloseAuctionResponse()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var bidderId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);
            var bidder = new Bidder { Name = "John Doe", Email = "johny@mail.com" };
            typeof(Bidder).GetProperty("Id")!.SetValue(bidder, bidderId);

            var highestBid = 9000m;

            var bid = new Bid(Guid.NewGuid(), auctionId, highestBid, bidder);

            var bidsList = new List<Bid?> { bid };

            var auction = new Auction(vehicle, AuctionStatus.OnGoing, bidsList, highestBid, bidder.Id);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);

            var closedAuction = new Auction(vehicle, AuctionStatus.Closed, bidsList, highestBid, bidder.Id);
            typeof(Auction).GetProperty("Id")!.SetValue(closedAuction, auctionId);

            var request = new CloseAuctionRequest
            {
                AuctionId = auctionId
            };

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            _auctionRepoMock
                .Setup(r => r.CloseAsync(auctionId))
                .ReturnsAsync(closedAuction);

            // Act
            var result = await _auctionService.CloseAuctionAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.AuctionId.Should().Be(auctionId);
            result.Status.Should().Be(AuctionStatus.Closed);
            result.FinalPrice.Should().Be(highestBid);
            result.WinnerId.Should().Be(bidder.Id);
        }

        [Fact]
        public async Task CloseAuctionAsync_AuctionNotFound_ThrowsAuctionNotFoundException()
        {
            // Arrange
            var request = new CloseAuctionRequest
            {
                AuctionId = Guid.NewGuid()
            };

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(request.AuctionId))
                .ReturnsAsync((Auction?)null);

            // Act
            Func<Task> act = async () => await _auctionService.CloseAuctionAsync(request);

            // Assert
            await act.Should().ThrowAsync<AuctionNotFoundException>();
        }

        [Fact]
        public async Task CloseAuctionAsync_AuctionNotActive_ThrowsAuctionNotActiveException()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);
            var auction = new Auction(vehicle, AuctionStatus.NotStarted, new List<Bid?>(), null, null);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);

            var request = new CloseAuctionRequest
            {
                AuctionId = auctionId
            };

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            // Act
            Func<Task> act = async () => await _auctionService.CloseAuctionAsync(request);

            // Assert
            await act.Should().ThrowAsync<AuctionNotActiveException>();
        }

        [Fact]
        public async Task CloseAuctionAsync_AuctionWithoutBids_ThrowsAuctionWithoutBidsException()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);
            var auction = new Auction(vehicle, AuctionStatus.OnGoing, new List<Bid?>(), null, null);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);

            var request = new CloseAuctionRequest
            {
                AuctionId = auctionId
            };

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            // Act
            Func<Task> act = async () => await _auctionService.CloseAuctionAsync(request);

            // Assert
            await act.Should().ThrowAsync<AuctionWithoutBidsException>();
        }
    }
}
