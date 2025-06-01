using FluentValidation;

namespace CarAuctionManagementSystem.Models.DTOs.Requests
{
    public class AddVehicleToAuctionRequest
    {
        public Guid AuctionId { get; set; }
        public Guid VehicleId { get; set; }
    }

    public class AddVehiclesToAuctionRequestValidator : AbstractValidator<AddVehicleToAuctionRequest>
    {
        public AddVehiclesToAuctionRequestValidator()
        {
            RuleFor(x => x.AuctionId)
                .NotEmpty()
                .WithMessage("AuctionId is required.");

            RuleFor(x => x.VehicleId)
                .NotEmpty()
                .WithMessage("VehicleId is required.");
        }
    }
}
