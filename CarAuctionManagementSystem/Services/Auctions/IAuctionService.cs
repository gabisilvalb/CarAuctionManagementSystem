using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;

namespace CarAuctionManagementSystem.Services.Auctions
{
    public interface IAuctionService
    {
        Task<AuctionResponse> AddAuctionAsync(Guid vehicleId);
        Task<StartAuctionResponse> StartAuctionAsync(Guid auctionId);
        Task<List<AuctionResponse>> GetAllAuctionsAsync();
        Task<List<AuctionResponse>> GetOnGoingAuctionsAsync();
        Task<PlaceBidResponse> PlaceBidAsync(PlaceBidRequest request);
        Task<CloseAuctionResponse> CloseAuctionAsync(CloseAuctionRequest request);
        Task<List<AuctionResponse>> GetAllClosedAuctionsAsync();
        Task<AuctionResponse> GetAuctionByIdAsync(Guid id);
        Task<AuctionBidsResponse> GetAuctionBidsAsync(Guid id);
    }
}
