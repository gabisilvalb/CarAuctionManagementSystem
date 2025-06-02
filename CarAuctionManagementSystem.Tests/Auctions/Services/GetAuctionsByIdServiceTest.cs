using CarAuctionManagement.Models.Auction;
using CarAuctionManagement.Models.Enums;
using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.Auction;
using CarAuctionManagementSystem.Models.Bidder;
using CarAuctionManagementSystem.Repositories.Auctions;
using CarAuctionManagementSystem.Repositories.Bidders;
using CarAuctionManagementSystem.Repositories.Vehicles;
using CarAuctionManagementSystem.Services.Auctions;
using FluentAssertions;
using Moq;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAuctionManagementSystem.Tests.Auctions.Services
{
    public class GetAuctionsByIdServiceTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepoMock;
        private readonly Mock<IVehicleRepository> _vehicleRepoMock;
        private readonly Mock<IBidderRepository> _bidderRepoMock;
        private readonly AuctionService _auctionService;

        public GetAuctionsByIdServiceTests()
        {
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _vehicleRepoMock = new Mock<IVehicleRepository>();
            _bidderRepoMock = new Mock<IBidderRepository>();
            _auctionService = new AuctionService(_vehicleRepoMock.Object, _auctionRepoMock.Object, _bidderRepoMock.Object);
        }

        [Fact]
        public async Task GetAuctionByIdAsync_ValidId_ReturnsAuctionResponse()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);
            var bidder = new Bidder { Name = "John Doe", Email = "johny@mail.com" };
            var highestBid = 9000m;
            var bid = new Bid(Guid.NewGuid(), auctionId, highestBid, bidder);
            var bidsList = new List<Bid?> { bid };

            var auction = new Auction(vehicle, AuctionStatus.OnGoing, bidsList, highestBid, bidder.Id);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);


            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            // Act
            var result = await _auctionService.GetAuctionByIdAsync(auctionId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(auctionId);
            result.Status.Should().Be(AuctionStatus.OnGoing);
            result.Vehicle.Should().Be(vehicle);
            result.HighestBid.Should().Be(highestBid);
            result.Bids.Should().BeEquivalentTo(bidsList);
            result.HighestBidder.Should().Be(bidder.Id);
        }

        [Fact]
        public async Task GetAuctionByIdAsync_AuctionNotFound_ThrowsAuctionNotFoundException()
        {
            // Arrange
            var auctionId = Guid.NewGuid();

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync((Auction?)null);

            // Act
            Func<Task> act = async () => await _auctionService.GetAuctionByIdAsync(auctionId);

            // Assert
            await act.Should().ThrowAsync<AuctionNotFoundException>();
        }
    }
}
