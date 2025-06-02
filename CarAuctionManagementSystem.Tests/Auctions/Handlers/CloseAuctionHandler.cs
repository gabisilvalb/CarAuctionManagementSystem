using CarAuctionManagement.Models.Enums;
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

public class CloseAuctionHandlerTests
{
    private readonly Mock<IAuctionService> _auctionServiceMock = new();
    private readonly Mock<IValidator<CloseAuctionRequest>> _validatorMock = new();

    [Fact]
    public async Task CloseAuction_ValidRequest_ReturnsOkWithResult()
    {
        // Arrange
        var request = new CloseAuctionRequest
        {
            AuctionId = Guid.NewGuid()
        };

        var expectedResponse = new CloseAuctionResponse
        {
            AuctionId = request.AuctionId,
            Status = AuctionStatus.Closed,
            FinalPrice = 5000,
            WinnerId = Guid.NewGuid()
        };


        _validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _auctionServiceMock
            .Setup(s => s.CloseAuctionAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await AuctionHandler.CloseAuction(request, _auctionServiceMock.Object, _validatorMock.Object);

        // Assert
        result.Should().NotBeNull();
        var resultType = result.GetType();
        resultType.Name.Should().StartWith("Ok");

        var value = resultType.GetProperty("Value")!.GetValue(result);
        value.Should().BeEquivalentTo(new { result = expectedResponse });
    }

    [Fact]
    public async Task CloseAuction_InvalidRequest_ReturnsValidationProblem()
    {
        // Arrange
        var request = new CloseAuctionRequest(); // Invalid request

        var validator = new CloseAuctionRequestValidator();
        var validationResult = await validator.ValidateAsync(request);

        _validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await AuctionHandler.CloseAuction(request, _auctionServiceMock.Object, _validatorMock.Object);

        // Assert
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>();

        var problemResult = result as Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult;
        var problemDetails = problemResult!.ProblemDetails as HttpValidationProblemDetails;

        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().ContainKey("AuctionId");
    }

}
