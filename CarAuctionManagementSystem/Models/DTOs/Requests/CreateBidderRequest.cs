using FluentValidation;

namespace CarAuctionManagementSystem.Models.DTOs.Requests
{
    public class CreateBidderRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
    public class CreateBidderRequestValidator : AbstractValidator<CreateBidderRequest>
    {
        public CreateBidderRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
        }
    }
}

