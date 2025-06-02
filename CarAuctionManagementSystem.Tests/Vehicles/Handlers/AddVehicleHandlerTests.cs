using CarAuctionManagement.Models.Enums;
using CarAuctionManagementSystem.Handlers;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Services.Vehicles;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;

namespace CarAuctionManagementSystem.Tests.Vehicles.Handlers
{
    public class AddVehicleHandlerTests
    {
        private readonly Mock<IVehicleService> _vehicleServiceMock;
        private readonly Mock<IValidator<AddVehicleRequest>> _validatorMock;

        public AddVehicleHandlerTests()
        {
            _vehicleServiceMock = new Mock<IVehicleService>();
            _validatorMock = new Mock<IValidator<AddVehicleRequest>>();
        }

        [Fact]
        public async Task AddVehicle_ValidRequest_ReturnsCreated()
        {
            // Arrange
            var request = new AddVehicleRequest
            {
                Manufacturer = "Ford",
                Model = "Focus",
                Year = 2021,
                StartingBid = 5000,
                Type = CarAuctionManagement.Models.Enums.VehicleType.Hatchback,
                NumberOfDoors = 5
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var expectedVehicle = new CarAuctionManagement.Models.Vehicles.Hatchback(
                request.Manufacturer!,
                request.Model!,
                request.Year,
                request.StartingBid,
                request.NumberOfDoors ?? 0
            );

            _vehicleServiceMock
                .Setup(s => s.AddVehicleAsync(request))
                .ReturnsAsync(expectedVehicle);

            // Act
            var result = await VehicleHandlers.AddVehicle(request, _vehicleServiceMock.Object, _validatorMock.Object);

            // Assert

            result.Should().NotBeNull();
            var value = result.GetType().GetProperty("Value")?.GetValue(result);
            value.Should().BeEquivalentTo(new { id = expectedVehicle.Id });
        }

        [Fact]
        public async Task AddVehicle_InvalidRequest_ReturnsValidationProblem()
        {
            // Arrange
            var request = new AddVehicleRequest
            {
                Manufacturer = "",
                Model = "",
                Year = 3000,
                Type = VehicleType.Hatchback,
                NumberOfDoors = 0
            };

            var validator = new AddVehicleRequestValidator();

            var validationResult = await validator.ValidateAsync(request);

            _validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            var result = await VehicleHandlers.AddVehicle(request, _vehicleServiceMock.Object, _validatorMock.Object);

            // Assert
            result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>();

            var problemResult = result as Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult;
            var problemDetails = problemResult!.ProblemDetails as HttpValidationProblemDetails;

            problemDetails.Should().NotBeNull();
            problemDetails!.Errors.Should().ContainKey("Manufacturer");
            problemDetails.Errors.Should().ContainKey("Model");
            problemDetails.Errors.Should().ContainKey("Year");
            problemDetails.Errors.Should().ContainKey("NumberOfDoors");
        }
    }
}
