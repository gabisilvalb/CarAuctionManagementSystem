using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;

namespace CarAuctionManagementSystem.Services.Bidders
{
    public interface IBidderService
    {
        Task<BidderDetailsResponse> GetBidderWithBidsAsync(Guid bidderId);
        Task<BidderAuctionsResponse> GetAuctionsByBidderAsync(Guid bidderId);
        Task DeleteBidderAsync(Guid id);
        Task<CreateBidderResponse> CreateBidderAsync(CreateBidderRequest request);
    }
}
