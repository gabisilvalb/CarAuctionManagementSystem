namespace CarAuctionManagementSystem.Models.DTOs.Responses
{
    public class BidderDetailsResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public List<BidHistoryDto> Bids { get; set; } = new();
    }

    public class BidHistoryDto
    {
        public Guid AuctionId { get; set; }
        public decimal Amount { get; set; }
    }
}
