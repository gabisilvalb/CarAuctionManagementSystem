using CarAuctionManagement.Models.Enums;

namespace CarAuctionManagement.Models.Vehicles
{
    public class Hatchback : Vehicle
    {
        public int? NumberOfDoors { get; set; }

        public Hatchback(string manufacturer, string model, int year, decimal startingBid, decimal currentBid, int numberOfDoors)
            : base(manufacturer, model, year, startingBid, currentBid, VehicleType.Truck)
        {
            NumberOfDoors = numberOfDoors;
        }
    }
}
