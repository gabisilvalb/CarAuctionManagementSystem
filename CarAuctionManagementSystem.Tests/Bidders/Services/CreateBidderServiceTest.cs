using CarAuctionManagementSystem.Models.Bidder;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;
using CarAuctionManagementSystem.Repositories.Auctions;
using CarAuctionManagementSystem.Repositories.Bidders;
using CarAuctionManagementSystem.Services.Bidders;
using FluentAssertions;
using Moq;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAuctionManagementSystem.Tests.Bidders.Services
{
    public class CreateBidderServiceTests
    {
        private readonly Mock<IBidderRepository> _bidderRepoMock;
        private readonly Mock<IAuctionRepository> _auctionRepoMock;
        private readonly BidderService _bidderService;

        public CreateBidderServiceTests()
        {
            _bidderRepoMock = new Mock<IBidderRepository>();
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _bidderService = new BidderService(_bidderRepoMock.Object, _auctionRepoMock.Object);
        }

        [Fact]
        public async Task CreateBidderAsync_ValidRequest_ReturnsCreateBidderResponse()
        {
            // Arrange
            var request = new CreateBidderRequest
            {
                Name = "John Doe",
                Email = "john@example.com"
            };

            _bidderRepoMock
                .Setup(r => r.EmailExistsAsync(request.Email!))
                .ReturnsAsync(false);

            var bidder = new Bidder
            {
                Name = request.Name,
                Email = request.Email
            };
            typeof(Bidder).GetProperty("Id")!.SetValue(bidder, Guid.NewGuid());

            _bidderRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Bidder>()))
                .ReturnsAsync(bidder);

            var expectedResponse = new CreateBidderResponse
            {
                Id = bidder.Id,
                Name = bidder.Name,
                Email = bidder.Email
            };

            // Act
            var result = await _bidderService.CreateBidderAsync(request);

            // Assert
            result.Should().BeEquivalentTo(expectedResponse);
        }
        [Fact]
        public async Task CreateBidderAsync_EmailAlreadyExists_ThrowsBidderAlreadyExistsException()
        {
            // Arrange
            var request = new CreateBidderRequest
            {
                Name = "Jane Smith",
                Email = "jane@example.com"
            };

            _bidderRepoMock
                .Setup(r => r.EmailExistsAsync(request.Email!))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _bidderService.CreateBidderAsync(request);

            // Assert
            await act.Should().ThrowAsync<BidderAlreadyExistsException>();
        }

    }
}
