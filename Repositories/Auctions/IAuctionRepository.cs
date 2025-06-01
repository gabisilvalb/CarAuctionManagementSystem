using CarAuctionManagement.Models.Auction;
using CarAuctionManagementSystem.Models.Auction;

namespace CarAuctionManagementSystem.Repositories.Auctions
{
    public interface IAuctionRepository
    {
        Task<bool> ExistsForVehicleAsync(Guid vehicleId);
        Task AddAsync(Auction auction);
        Task<Auction?> GetByIdAsync(Guid id);
        Task<Auction> StartAuctionAsync(Auction auction);
        Task<List<Auction>> GetAllAsync();
        Task<Bid> PlaceBidAsync(Guid auctionId, Bid bid);
    }
}
