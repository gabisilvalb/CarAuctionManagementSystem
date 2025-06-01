namespace CarAuctionManagementSystem.Models.DTOs.Responses
{
    public class GetAllOnGoingAuctionsResponse
    {
        public List<AuctionResponse> Auctions { get; set; } = new();
    }
}
