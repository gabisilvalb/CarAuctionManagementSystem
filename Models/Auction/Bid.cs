namespace CarAuctionManagementSystem.Models.Auction
{
    public class Bid
    {
        public Guid? Id { get; }
        public Guid AuctionId { get; }
        public decimal? Amount { get; }
        public Bidder.Bidder Bidder { get; }

        public Bid(Guid? id, Guid auctionId, decimal? amount, Bidder.Bidder bidder)
        {
            Id = id;
            AuctionId = auctionId;
            Amount = amount;
            Bidder = bidder;
        }
    }
}
