namespace CarAuctionManagementSystem.Models.Bidder
{
    public class Bidder
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public string? Email { get; set; }
    }

}
