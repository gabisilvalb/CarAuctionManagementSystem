using CarAuctionManagement.Models.Enums;

namespace CarAuctionManagementSystem.Models.DTOs.Responses
{
    public class StartAuctionResponse
    {
        public Guid AuctionId { get; set; }
        public AuctionStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
    }
}
