using CarAuctionManagement.Models.Enums;

namespace CarAuctionManagementSystem.Models.DTOs.Responses
{
    public class CloseAuctionResponse
    {
        public Guid AuctionId { get; set; }
        public AuctionStatus Status { get; set; }
        public decimal? FinalPrice { get; set; }
        public Guid? WinnerId { get; set; }
    }

}
