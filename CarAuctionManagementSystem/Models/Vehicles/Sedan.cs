using CarAuctionManagement.Models.Enums;

namespace CarAuctionManagement.Models.Vehicles
{
    public class Sedan : Vehicle
    {
        public int? NumberOfDoors { get; set; }

        public Sedan(string manufacturer, string model, int year, decimal startingBid, int numberOfDoors)
            : base(manufacturer, model, year, startingBid, VehicleType.Sedan)
        {
            NumberOfDoors = numberOfDoors;
        }
    }
}
