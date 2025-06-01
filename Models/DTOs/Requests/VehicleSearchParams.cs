using CarAuctionManagement.Models.Enums;

namespace CarAuctionManagementSystem.Models.DTOs.Requests
{
    public class VehicleSearchParams
    {
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public VehicleType? Type { get; set; }
        public int? Year { get; set; }
        public decimal? StartingBid { get; set; }
    }
}
