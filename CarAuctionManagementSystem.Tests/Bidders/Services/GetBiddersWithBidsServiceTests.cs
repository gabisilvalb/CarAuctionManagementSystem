using CarAuctionManagement.Models.Auction;
using CarAuctionManagement.Models.Enums;
using CarAuctionManagementSystem.Models.Auction;
using CarAuctionManagementSystem.Models.Bidder;
using CarAuctionManagementSystem.Repositories.Auctions;
using CarAuctionManagementSystem.Repositories.Bidders;
using CarAuctionManagementSystem.Services.Bidders;
using FluentAssertions;
using Moq;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAuctionManagementSystem.Tests.Bidders.Services
{
    public class GetBidderWithBidsServiceTests
    {
        private readonly Mock<IBidderRepository> _bidderRepoMock;
        private readonly Mock<IAuctionRepository> _auctionRepoMock;
        private readonly BidderService _bidderService;

        public GetBidderWithBidsServiceTests()
        {
            _bidderRepoMock = new Mock<IBidderRepository>();
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _bidderService = new BidderService(_bidderRepoMock.Object, _auctionRepoMock.Object);
        }

        [Fact]
        public async Task GetBidderWithBidsAsync_ValidBidderId_ReturnsBidderDetailsResponse()
        {
            // Arrange
            var bidderId = Guid.NewGuid();
            var bidder = new Bidder { Name = "Alice", Email = "alice@example.com" };
            typeof(Bidder).GetProperty("Id")!.SetValue(bidder, bidderId);

            _bidderRepoMock
                .Setup(r => r.GetByIdAsync(bidderId))
                .ReturnsAsync(bidder);

            var bid1 = new Bid(Guid.NewGuid(), Guid.NewGuid(), 1000m, bidder);
            var bid2 = new Bid(Guid.NewGuid(), Guid.NewGuid(), 1500m, bidder);

            var auction1 = new Auction(null, AuctionStatus.Closed, new List<Bid?> { bid1 }, 1000m, bidderId);
            var auction2 = new Auction(null, AuctionStatus.Closed, new List<Bid?> { bid2 }, 1500m, bidderId);

            _auctionRepoMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Auction> { auction1, auction2 });

            // Act
            var result = await _bidderService.GetBidderWithBidsAsync(bidderId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(bidderId);
            result.Name.Should().Be("Alice");
            result.Email.Should().Be("alice@example.com");
            result.Bids.Should().HaveCount(2);
            result.Bids.Should().Contain(b => b.AuctionId == bid1.AuctionId && b.Amount == bid1.Amount);
            result.Bids.Should().Contain(b => b.AuctionId == bid2.AuctionId && b.Amount == bid2.Amount);
        }
        [Fact]
        public async Task GetBidderWithBidsAsync_BidderNotFound_ThrowsBidderNotFoundByIdException()
        {
            // Arrange
            var bidderId = Guid.NewGuid();

            _bidderRepoMock
                .Setup(r => r.GetByIdAsync(bidderId))
                .ReturnsAsync((Bidder?)null);

            // Act
            Func<Task> act = async () => await _bidderService.GetBidderWithBidsAsync(bidderId);

            // Assert
            await act.Should().ThrowAsync<BidderNotFoundByIdException>();
        }

    }
}
