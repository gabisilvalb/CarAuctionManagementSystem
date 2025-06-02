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
    public class GetAuctionBidsServiceTest
    {
        private readonly Mock<IAuctionRepository> _auctionRepoMock;
        private readonly Mock<IVehicleRepository> _vehicleRepoMock;
        private readonly Mock<IBidderRepository> _bidderRepoMock;
        private readonly AuctionService _auctionService;

        public GetAuctionBidsServiceTest()
        {
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _vehicleRepoMock = new Mock<IVehicleRepository>();
            _bidderRepoMock = new Mock<IBidderRepository>();
            _auctionService = new AuctionService(_vehicleRepoMock.Object, _auctionRepoMock.Object, _bidderRepoMock.Object);
        }

        [Fact]
        public async Task GetAuctionBidsAsync_ValidAuction_ReturnsAuctionBidsResponse()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);
            var bidder = new Bidder { Name = "John Doe", Email = "johny@mail.com" };
            var bid = new Bid(Guid.NewGuid(), auctionId, 9000m, bidder);
            var bidsList = new List<Bid?> { bid };

            var auction = new Auction(vehicle, AuctionStatus.OnGoing, bidsList, 9000m, bidder.Id);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);


            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            // Act
            var result = await _auctionService.GetAuctionBidsAsync(auctionId);

            // Assert
            result.Should().NotBeNull();
            result.AuctionId.Should().Be(auctionId);
            result.Bids.Should().HaveCount(1);
            result.Bids[0].BidId.Should().Be(bid.Id!.Value);
            result.Bids[0].Bidder.Should().Be(bid.Bidder);
            result.Bids[0].Amount.Should().Be(bid.Amount!.Value);
        }

        [Fact]
        public async Task GetAuctionBidsAsync_AuctionNotFound_ThrowsAuctionNotFoundException()
        {
            // Arrange
            var auctionId = Guid.NewGuid();

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync((Auction?)null);

            // Act
            Func<Task> act = async () => await _auctionService.GetAuctionBidsAsync(auctionId);

            // Assert
            await act.Should().ThrowAsync<AuctionNotFoundException>();
        }
    }
}
