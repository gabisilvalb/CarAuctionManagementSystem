using CarAuctionManagementSystem.Models.Bidder;

namespace CarAuctionManagementSystem.Repositories.Bidders
{
    public interface IBidderRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task DeleteAsync(Guid id);
        Task<Bidder> AddAsync(Bidder bidder);
        Task<Bidder?> GetByIdAsync(Guid id);
    }

}
