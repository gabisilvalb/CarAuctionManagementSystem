using FluentValidation;

namespace CarAuctionManagementSystem.Models.DTOs.Requests
{
    public class PlaceBidRequest
    {
        public Guid AuctionId { get; set; }
        public Guid BidderId { get; set; }
        public decimal Amount { get; set; }
    }
    public class PlaceBidRequestValidator : AbstractValidator<PlaceBidRequest>
    {
        public PlaceBidRequestValidator()
        {
            RuleFor(x => x.AuctionId)
                .NotEmpty()
                .WithMessage("AuctionId is required.");

            RuleFor(x => x.BidderId)
                .NotEmpty()
                .WithMessage("BidderId is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Bid amount must be greater than zero.");
        }
    }
}
