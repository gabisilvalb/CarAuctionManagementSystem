using CarAuctionManagement.Models.Enums;

namespace CarAuctionManagementSystem.Models.DTOs.Responses
{
    public class VehicleResponse
    {
        public Guid Id { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public decimal? StartingBid { get; set; }
        public decimal? CurrentBid { get; set; }
        public bool IsAuctionActive { get; set; }
        public VehicleType? Type { get; set; }

        // Optional type-specific fields
        public int? NumberOfDoors { get; set; }
        public int? NumberOfSeats { get; set; }
        public decimal? LoadCapacity { get; set; }
    }
}
