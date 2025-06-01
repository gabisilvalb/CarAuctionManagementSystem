namespace CarAuctionManagementSystem.Models.DTOs.Responses
{
    public class CreateBidderResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}
