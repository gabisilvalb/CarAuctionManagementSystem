namespace CarAuctionManagementSystem.Models.DTOs.Responses
{
    public class GetAllClosedAuctionsResponse
    {
        public List<AuctionResponse> Auctions { get; set; } = new();
    }
}
