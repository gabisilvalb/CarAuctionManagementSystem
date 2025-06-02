namespace CarAuctionManagementSystem.Models.DTOs.Responses
{
    public class AuctionBidsResponse
    {
        public Guid AuctionId { get; set; }
        public List<BidDto> Bids { get; set; } = new();
    }
    public class BidDto
    {
        public Guid BidId { get; set; }
        public Bidder.Bidder? Bidder { get; set; }
        public decimal Amount { get; set; }
    }
}
