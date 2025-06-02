using CarAuctionManagementSystem.Handlers;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;
using CarAuctionManagementSystem.Services.Auctions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;

namespace CarAutionManagementSystem.Tests.Auctions.Handlers;

public class PlaceBidHandlerTests
{
    private readonly Mock<IAuctionService> _auctionServiceMock = new();
    private readonly Mock<IValidator<PlaceBidRequest>> _validatorMock = new();

    [Fact]
    public async Task PlaceBid_ValidRequest_ReturnsOkWithResult()
    {
        // Arrange
        var request = new PlaceBidRequest
        {
            AuctionId = Guid.NewGuid(),
            BidderId = Guid.NewGuid(),
            Amount = 1000
        };

        var expectedResponse = new PlaceBidResponse
        {
            AuctionId = request.AuctionId,
            BidId = Guid.NewGuid(),
            Amount = request.Amount,
            BidderId = request.BidderId,
            BidderName = "John Doe"
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _auctionServiceMock
            .Setup(s => s.PlaceBidAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await AuctionHandler.PlaceBid(request, _auctionServiceMock.Object, _validatorMock.Object);

        // Assert
        result.Should().NotBeNull();

        var resultType = result.GetType();
        resultType.Name.Should().StartWith("Ok");

        var value = resultType.GetProperty("Value")!.GetValue(result);

        value.Should().BeEquivalentTo(new { result = expectedResponse });
    }

    [Fact]
    public async Task PlaceBid_InvalidRequest_ReturnsValidationProblem()
    {
        // Arrange
        var request = new PlaceBidRequest(); // invalid

        var validator = new PlaceBidRequestValidator();

        var validationResult = await validator.ValidateAsync(request);

        _validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await AuctionHandler.PlaceBid(request, _auctionServiceMock.Object, _validatorMock.Object);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>();

        var problemResult = result as Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult;
        var problemDetails = problemResult!.ProblemDetails as HttpValidationProblemDetails;

        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().ContainKey("AuctionId");
        problemDetails.Errors.Should().ContainKey("BidderId");
        problemDetails.Errors.Should().ContainKey("Amount");
    }
}
