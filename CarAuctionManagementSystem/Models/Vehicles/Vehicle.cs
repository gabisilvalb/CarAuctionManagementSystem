using CarAuctionManagement.Models.Enums;

namespace CarAuctionManagement.Models.Vehicles
{
    public class Vehicle
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public decimal? StartingBid { get; set; }
        public VehicleType? Type { get; set; }

        protected Vehicle(string manufacturer, string model, int year, decimal startingBid, VehicleType type)
        {
            Manufacturer = manufacturer;
            Model = model;
            Year = year;
            StartingBid = startingBid;
            Type = type;
        }
    }
}
