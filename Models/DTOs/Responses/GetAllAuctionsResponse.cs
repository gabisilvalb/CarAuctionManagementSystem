namespace CarAuctionManagementSystem.Models.DTOs.Responses
{
    public class GetAllAuctionsResponse
    {
        public List<AuctionResponse> Auctions { get; set; } = new();
    }
}
