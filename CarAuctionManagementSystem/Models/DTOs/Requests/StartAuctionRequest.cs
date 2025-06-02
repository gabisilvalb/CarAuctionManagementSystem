using FluentValidation;

namespace CarAuctionManagementSystem.Models.DTOs.Requests
{
    public class StartAuctionRequest
    {
        public Guid AuctionId { get; set; }
    }
    public class StartAuctionRequestValidator : AbstractValidator<StartAuctionRequest>
    {
        public StartAuctionRequestValidator()
        {
            RuleFor(x => x.AuctionId)
                .NotEmpty()
                .WithMessage("AuctionId is required to start an auction.");
        }
    }
}
