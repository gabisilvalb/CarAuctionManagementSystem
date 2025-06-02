using CarAuctionManagementSystem.Handlers;
using CarAuctionManagementSystem.Models.Bidder;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;
using CarAuctionManagementSystem.Services.Bidders;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;

namespace CarAuctionManagementSystem.Tests.Bidders.Handlers
{
    public class CreateBidderHandlerTests
    {
        private readonly Mock<IValidator<CreateBidderRequest>> _validatorMock;
        private readonly Mock<IBidderService> _serviceMock;

        public CreateBidderHandlerTests()
        {
            _validatorMock = new Mock<IValidator<CreateBidderRequest>>();
            _serviceMock = new Mock<IBidderService>();
        }

        [Fact]
        public async Task CreateBidder_ValidRequest_ReturnsCreated()
        {
            // Arrange

            var bidderId = Guid.NewGuid();
            var request = new CreateBidderRequest
            {
                Name = "John Doe",
                Email = "john@example.com"
            };

            var bidder = new Bidder { Name = request.Name, Email = request.Email };
            typeof(Bidder).GetProperty("Id")!.SetValue(bidder, bidderId);

            var expectedResponse = new CreateBidderResponse
            {
                Id = bidderId,
                Name = request.Name,
                Email = request.Email
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _serviceMock
                .Setup(s => s.CreateBidderAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await BidderHandler.CreateBidder(request, _validatorMock.Object, _serviceMock.Object);

            // Assert
            result.Should().NotBeNull();
            var value = result.GetType().GetProperty("Value")?.GetValue(result);
            var location = result.GetType().GetProperty("Location")?.GetValue(result);
            value.Should().BeEquivalentTo(new { result = expectedResponse });
            location.Should().BeEquivalentTo($"/bidders/{expectedResponse.Id}");
        }

        [Fact]
        public async Task CreateBidder_InvalidRequest_ReturnsValidationProblem()
        {
            // Arrange
            var request = new CreateBidderRequest(); // Missing name & email

            var validator = new CreateBidderRequestValidator();

            var validationResult = await validator.ValidateAsync(request);

            _validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await BidderHandler.CreateBidder(request, _validatorMock.Object, _serviceMock.Object);

            // Assert
            result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>();

            var problemResult = result as Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult;
            var problemDetails = problemResult!.ProblemDetails as HttpValidationProblemDetails;

            problemDetails!.Errors.Should().ContainKey("Name");
            problemDetails.Errors.Should().ContainKey("Email");
        }
    }
}
