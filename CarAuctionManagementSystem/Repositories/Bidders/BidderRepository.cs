using CarAuctionManagementSystem.Models.Bidder;

namespace CarAuctionManagementSystem.Repositories.Bidders
{
    public class BidderRepository : IBidderRepository
    {
        public Task<bool> EmailExistsAsync(string email)
        {
            var exists = _bidders.Any(b =>
                b.Email!.Equals(email, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(exists);
        }

        private readonly List<Bidder> _bidders = new();
        public Task DeleteAsync(Guid id)
        {
            var bidder = _bidders.FirstOrDefault(b => b.Id == id);
            _bidders.Remove(bidder!);

            return Task.CompletedTask;
        }
        public Task<Bidder> AddAsync(Bidder bidder)
        {
            _bidders.Add(bidder);
            return Task.FromResult(bidder);
        }

        public Task<Bidder?> GetByIdAsync(Guid id)
        {
            var bidder = _bidders.FirstOrDefault(b => b.Id == id);
            return Task.FromResult(bidder);
        }
    }

}
