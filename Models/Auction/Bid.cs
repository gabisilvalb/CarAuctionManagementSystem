namespace CarAuctionManagementSystem.Models.Auction
{
    public class Bid
    {
        public Guid? Id { get; }
        public Guid? BidderId { get; }
        public Guid? AuctionId { get; }
        public decimal? Amount { get; }

        public Bid(Guid? id, Guid? bidderId, Guid? auctionId, decimal? amount)
        {
            Id = id;
            BidderId = bidderId;
            AuctionId = auctionId;
            Amount = amount;
        }
    }
}
