using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Services.Bidders;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace CarAuctionManagementSystem.Handlers
{
    public class BidderHandler
    {
        public static async Task<IResult> DeleteBidder(Guid id, IBidderService service)
        {
            await service.DeleteBidderAsync(id);
            return Results.NoContent();
        }

        public static async Task<IResult> CreateBidder(CreateBidderRequest request,
                                                IValidator<CreateBidderRequest> validator,
                                                IBidderService service)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var result = await service.CreateBidderAsync(request);
            return Results.Created($"/bidders/{result.Id}", new { result });
        }

        public static async Task<IResult> GetAuctionsByBidder(
            Guid id,
            IBidderService bidderService)
        {
            var result = await bidderService.GetAuctionsByBidderAsync(id);
            return Results.Ok(new { result });
        }

        public static async Task<IResult> GetBidderById(
            Guid id,
            IBidderService bidderService)
        {
            var result = await bidderService.GetBidderWithBidsAsync(id);
            return Results.Ok(new { result });
        }

    }
}
