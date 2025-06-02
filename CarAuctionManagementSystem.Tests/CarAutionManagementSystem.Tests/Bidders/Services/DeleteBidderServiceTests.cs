using CarAuctionManagementSystem.Models.Bidder;
using CarAuctionManagementSystem.Repositories.Auctions;
using CarAuctionManagementSystem.Repositories.Bidders;
using CarAuctionManagementSystem.Services.Bidders;
using FluentAssertions;
using Moq;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAuctionManagementSystem.Tests.Bidders.Services
{
    public class DeleteBidderServiceTests
    {
        private readonly Mock<IBidderRepository> _bidderRepoMock;
        private readonly Mock<IAuctionRepository> _auctionRepoMock;
        private readonly BidderService _bidderService;

        public DeleteBidderServiceTests()
        {
            _bidderRepoMock = new Mock<IBidderRepository>();
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _bidderService = new BidderService(_bidderRepoMock.Object, _auctionRepoMock.Object);
        }

        [Fact]
        public async Task DeleteBidderAsync_ValidId_DeletesBidder()
        {
            // Arrange
            var bidderId = Guid.NewGuid();
            var bidder = new Bidder { Name = "Test User", Email = "test@example.com" };
            typeof(Bidder).GetProperty("Id")!.SetValue(bidder, bidderId);

            _bidderRepoMock
                .Setup(r => r.GetByIdAsync(bidderId))
                .ReturnsAsync(bidder);

            _auctionRepoMock
                .Setup(r => r.HasBidsByBidderAsync(bidderId))
                .ReturnsAsync(false);

            // Act
            await _bidderService.DeleteBidderAsync(bidderId);

            // Assert
            _bidderRepoMock.Verify(r => r.DeleteAsync(bidderId), Times.Once);
        }

        [Fact]
        public async Task DeleteBidderAsync_BidderNotFound_ThrowsException()
        {
            // Arrange
            var bidderId = Guid.NewGuid();

            _bidderRepoMock
                .Setup(r => r.GetByIdAsync(bidderId))
                .ReturnsAsync((Bidder?)null);

            // Act
            Func<Task> act = async () => await _bidderService.DeleteBidderAsync(bidderId);

            // Assert
            await act.Should().ThrowAsync<BidderNotFoundByIdException>();
        }

        [Fact]
        public async Task DeleteBidderAsync_BidderHasPlacedBids_ThrowsException()
        {
            // Arrange
            var bidderId = Guid.NewGuid();
            var bidder = new Bidder { Name = "Bidding User", Email = "bid@example.com" };
            typeof(Bidder).GetProperty("Id")!.SetValue(bidder, bidderId);

            _bidderRepoMock
                .Setup(r => r.GetByIdAsync(bidderId))
                .ReturnsAsync(bidder);

            _auctionRepoMock
                .Setup(r => r.HasBidsByBidderAsync(bidderId))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _bidderService.DeleteBidderAsync(bidderId);

            // Assert
            await act.Should().ThrowAsync<BidderHasPlacedBidsException>();
        }
    }
}
