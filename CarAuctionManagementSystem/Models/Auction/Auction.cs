using CarAuctionManagement.Models.Enums;
using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.Auction;

namespace CarAuctionManagement.Models.Auction
{
    public class Auction
    {

        public Guid Id { get; protected set; } = Guid.NewGuid();
        public Vehicle? Vehicle { get; set; }
        public AuctionStatus? Status { get; set; } = AuctionStatus.NotStarted;
        public List<Bid?>? Bids { get; set; }
        public decimal? HighestBid { get; set; }
        public Guid? HighestBidder { get; set; }
        public Auction(Vehicle? vehicle, AuctionStatus status, List<Bid?>? bids, decimal? highestBid, Guid? highestBidder)
        {
            Vehicle = vehicle;
            Status = status;
            Bids = bids;
            HighestBid = highestBid;
            HighestBidder = highestBidder;
        }
    }
}
