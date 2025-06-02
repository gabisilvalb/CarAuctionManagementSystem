using FluentValidation;

namespace CarAuctionManagementSystem.Models.DTOs.Requests
{
    public class AddAuctionRequest
    {
        public Guid VehicleId { get; set; }
    }

    public class AddAuctionRequestValidator : AbstractValidator<AddAuctionRequest>
    {
        public AddAuctionRequestValidator()
        {
            RuleFor(x => x.VehicleId)
                .NotEmpty()
                .WithMessage("VehicleId is required to create an auction.");
        }
    }
}
