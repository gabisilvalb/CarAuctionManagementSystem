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
    public class PlaceBidServiceTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepoMock;
        private readonly Mock<IBidderRepository> _bidderRepoMock;
        private readonly Mock<IVehicleRepository> _vehicleRepository;
        private readonly AuctionService _auctionService;

        public PlaceBidServiceTests()
        {
            _auctionRepoMock = new Mock<IAuctionRepository>();
            _bidderRepoMock = new Mock<IBidderRepository>();
            _vehicleRepository = new Mock<IVehicleRepository>();
            _auctionService = new AuctionService(_vehicleRepository.Object, _auctionRepoMock.Object, _bidderRepoMock.Object);
        }

        [Fact]
        public async Task PlaceBidAsync_ValidRequest_ReturnsPlaceBidResponse()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var bidderId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);
            var bidder = new Bidder { Name = "John Doe", Email = "johny@mail.com" };
            typeof(Bidder).GetProperty("Id")!.SetValue(bidder, bidderId);
            var auction = new Auction(vehicle, AuctionStatus.OnGoing, new List<Bid?>(), null, null);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);

            var request = new PlaceBidRequest
            {
                AuctionId = auctionId,
                BidderId = bidderId,
                Amount = 9000
            };
            var bid = new Bid(Guid.NewGuid(), auctionId, request.Amount, bidder);

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            _bidderRepoMock
                .Setup(r => r.GetByIdAsync(bidderId))
                .ReturnsAsync(bidder);

            _auctionRepoMock
                .Setup(r => r.PlaceBidAsync(auctionId, It.IsAny<Bid>()))
                .ReturnsAsync(bid);

            // Act
            var result = await _auctionService.PlaceBidAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.AuctionId.Should().Be(auctionId);
            result.BidId.Should().Be(bid.Id);
            result.Amount.Should().Be(request.Amount);
            result.BidderId.Should().Be(bidderId);
            result.BidderName.Should().Be(bidder.Name);
        }

        [Fact]
        public async Task PlaceBidAsync_AuctionNotFound_ThrowsAuctionNotFoundException()
        {
            // Arrange
            var request = new PlaceBidRequest
            {
                AuctionId = Guid.NewGuid(),
                BidderId = Guid.NewGuid(),
                Amount = 9000
            };

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(request.AuctionId))
                .ReturnsAsync((Auction?)null);

            // Act
            Func<Task> act = async () => await _auctionService.PlaceBidAsync(request);

            // Assert
            await act.Should().ThrowAsync<AuctionNotFoundException>();
        }

        [Fact]
        public async Task PlaceBidAsync_AuctionNotActive_ThrowsAuctionNotActiveException()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);
            var auction = new Auction(vehicle, AuctionStatus.NotStarted, new List<Bid?>(), null, null);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);

            var request = new PlaceBidRequest
            {
                AuctionId = auctionId,
                BidderId = Guid.NewGuid(),
                Amount = 9000
            };

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            // Act
            Func<Task> act = async () => await _auctionService.PlaceBidAsync(request);

            // Assert
            await act.Should().ThrowAsync<AuctionNotActiveException>();
        }

        [Fact]
        public async Task PlaceBidAsync_AuctionVehicleIsNull_ThrowsAuctionDoestHaveVehicleException()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var auction = new Auction(null!, AuctionStatus.OnGoing, new List<Bid?>(), null, null);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);

            var request = new PlaceBidRequest
            {
                AuctionId = auctionId,
                BidderId = Guid.NewGuid(),
                Amount = 9000
            };

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            // Act
            Func<Task> act = async () => await _auctionService.PlaceBidAsync(request);

            // Assert
            await act.Should().ThrowAsync<AuctionDoestHaveVehicleException>();
        }

        [Fact]
        public async Task PlaceBidAsync_BidAmountLowerThanStartingPrice_ThrowsBidAmountLowerThanStartingPriceException()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);
            var auction = new Auction(vehicle, AuctionStatus.OnGoing, new List<Bid?>(), null, null);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);

            var request = new PlaceBidRequest
            {
                AuctionId = auctionId,
                BidderId = Guid.NewGuid(),
                Amount = 7000
            };

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            // Act
            Func<Task> act = async () => await _auctionService.PlaceBidAsync(request);

            // Assert
            await act.Should().ThrowAsync<BidAmountLowerThanStartingPriceException>();
        }

        [Fact]
        public async Task PlaceBidAsync_BidderNotFound_ThrowsBidderNotFoundByIdException()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var bidderId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);
            var auction = new Auction(vehicle, AuctionStatus.OnGoing, new List<Bid?>(), null, null);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);

            var request = new PlaceBidRequest
            {
                AuctionId = auctionId,
                BidderId = bidderId,
                Amount = 9000
            };

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            _bidderRepoMock
                .Setup(r => r.GetByIdAsync(bidderId))
                .ReturnsAsync((Bidder?)null);

            // Act
            Func<Task> act = async () => await _auctionService.PlaceBidAsync(request);

            // Assert
            await act.Should().ThrowAsync<BidderNotFoundByIdException>();
        }

        [Fact]
        public async Task PlaceBidAsync_BidAmountTooLow_ThrowsBidAmountTooLowException()
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var bidderId = Guid.NewGuid();
            var vehicle = new Hatchback("Toyota", "Yaris", 2022, 8000, 4);
            var bidder = new Bidder { Name = "John Doe", Email = "johny@mail.com" };
            typeof(Bidder).GetProperty("Id")!.SetValue(bidder, bidderId);
            var existingBid = new Bid(Guid.NewGuid(), auctionId, 9000, bidder);
            var auction = new Auction(vehicle, AuctionStatus.OnGoing, new List<Bid?> { existingBid }, null, null);
            typeof(Auction).GetProperty("Id")!.SetValue(auction, auctionId);

            var request = new PlaceBidRequest
            {
                AuctionId = auctionId,
                BidderId = bidderId,
                Amount = 8500
            };

            _auctionRepoMock
                .Setup(r => r.GetByIdAsync(auctionId))
                .ReturnsAsync(auction);

            _bidderRepoMock
                .Setup(r => r.GetByIdAsync(bidderId))
                .ReturnsAsync(bidder);

            // Act
            Func<Task> act = async () => await _auctionService.PlaceBidAsync(request);

            // Assert
            await act.Should().ThrowAsync<BidAmountTooLowException>();
        }
    }
}
