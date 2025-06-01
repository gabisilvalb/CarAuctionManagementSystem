using CarAuctionManagement.Models.Enums;

namespace CarAuctionManagement.Models.Vehicles
{
    public class Truck : Vehicle
    {
        public decimal? LoadCapacity { get; set; }

        public Truck(string manufacturer, string model, int year, decimal startingBid, decimal currentBid, decimal loadCapacity)
            : base(manufacturer, model, year, startingBid, currentBid, VehicleType.Truck)
        {
            LoadCapacity = loadCapacity;
        }
    }
}
