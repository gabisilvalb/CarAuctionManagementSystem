using CarAuctionManagement.Models.Enums;

namespace CarAuctionManagement.Models.Vehicles
{
    public class Hatchback : Vehicle
    {
        public int? NumberOfDoors { get; set; }

        public Hatchback(string manufacturer, string model, int year, decimal startingBid, int numberOfDoors)
            : base(manufacturer, model, year, startingBid, VehicleType.Hatchback)
        {
            NumberOfDoors = numberOfDoors;
        }
    }
}
