using CarAuctionManagement.Models.Enums;
using CarAuctionManagementSystem.Handlers;
using CarAuctionManagementSystem.Models.Auction;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;
using CarAuctionManagementSystem.Services.Auctions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;

namespace CarAutionManagementSystem.Tests.Auctions.Handlers;

public class AddAuctionHandlerTests
{
    private readonly Mock<IAuctionService> _auctionServiceMock;
    private readonly Mock<IValidator<AddAuctionRequest>> _validatorMock;

    public AddAuctionHandlerTests()
    {
        _auctionServiceMock = new Mock<IAuctionService>();
        _validatorMock = new Mock<IValidator<AddAuctionRequest>>();
    }

    [Fact]
    public async Task AddAuction_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new AddAuctionRequest { VehicleId = Guid.NewGuid() };
        var expectedAuction = new AuctionResponse
        {
            Id = Guid.NewGuid(),
            Status = AuctionStatus.NotStarted,
            Vehicle = null,
            Bids = new List<Bid?>(),
            HighestBid = null,
            HighestBidder = null
        };


        _validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _auctionServiceMock
            .Setup(s => s.AddAuctionAsync(request.VehicleId))
            .ReturnsAsync(expectedAuction);

        // Act
        var result = await AuctionHandler.AddAuction(request, _auctionServiceMock.Object, _validatorMock.Object);

        // Assert
        result.Should().NotBeNull();

        // Use reflection to check the Created result
        var resultType = result.GetType();
        resultType.Name.Contains("Created").Should().BeTrue();

        var locationProperty = resultType.GetProperty("Location");
        var valueProperty = resultType.GetProperty("Value");

        locationProperty.Should().NotBeNull();
        valueProperty.Should().NotBeNull();

        var location = locationProperty!.GetValue(result) as string;
        var value = valueProperty!.GetValue(result);

        location.Should().Be($"/auctions/{expectedAuction.Id}");
        value.Should().BeEquivalentTo(new { auction = expectedAuction });
    }

    [Fact]
    public async Task AddAuction_InvalidRequest_ReturnsValidationProblem()
    {
        // Arrange
        var request = new AddAuctionRequest { VehicleId = Guid.Empty };

        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("VehicleId", "VehicleId is required to create an auction.")
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await AuctionHandler.AddAuction(request, _auctionServiceMock.Object, _validatorMock.Object);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>();

        var problemResult = result as Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult;
        var problemDetails = problemResult!.ProblemDetails as HttpValidationProblemDetails;

        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().ContainKey("VehicleId");
    }
}
