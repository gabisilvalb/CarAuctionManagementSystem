using CarAuctionManagement.Models.Enums;
using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.Auction;

namespace CarAuctionManagementSystem.Models.DTOs.Responses
{
    public class AuctionResponse
    {
        public Guid Id { get; set; }
        public AuctionStatus Status { get; set; }
        public Vehicle? Vehicle { get; set; }
        public List<Bid?>? Bids { get; set; }
        public decimal? HighestBid { get; set; }
        public Guid? HighestBidder { get; set; }
    }
}
