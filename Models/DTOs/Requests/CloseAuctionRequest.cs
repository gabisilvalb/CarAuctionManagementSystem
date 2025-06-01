using FluentValidation;

namespace CarAuctionManagementSystem.Models.DTOs.Requests
{
    public class CloseAuctionRequest
    {
        public Guid AuctionId { get; set; }
    }
    public class CloseAuctionRequestValidator : AbstractValidator<CloseAuctionRequest>
    {
        public CloseAuctionRequestValidator()
        {
            RuleFor(x => x.AuctionId)
                .NotEmpty()
                .WithMessage("AuctionId is required.");
        }
    }
}
