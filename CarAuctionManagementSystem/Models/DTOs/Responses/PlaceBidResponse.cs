namespace CarAuctionManagementSystem.Models.DTOs.Responses
{
    public class PlaceBidResponse
    {
        public Guid AuctionId { get; set; }
        public Guid? BidId { get; set; }
        public decimal Amount { get; set; }
        public Guid BidderId { get; set; }
        public string? BidderName { get; set; }
    }
}
