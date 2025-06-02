using CarAuctionManagement.Models.Enums;

namespace CarAuctionManagementSystem.Models.DTOs.Responses
{
    public class BidderAuctionsResponse
    {
        public Guid BidderId { get; set; }
        public List<BidderAuctionDto> Auctions { get; set; } = new();
    }

    public class BidderAuctionDto
    {
        public Guid AuctionId { get; set; }
        public AuctionStatus Status { get; set; }
        public Guid? VehicleId { get; set; }
        public int TotalBids { get; set; }
        public decimal? LastBidAmount { get; set; }
    }
}
