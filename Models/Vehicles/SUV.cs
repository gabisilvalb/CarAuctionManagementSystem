using CarAuctionManagement.Models.Enums;

namespace CarAuctionManagement.Models.Vehicles
{
    public class SUV : Vehicle
    {
        public int? NumberOfSeats { get; set; }

        public SUV(string manufacturer, string model, int year, decimal startingBid, decimal currentBid, int numberOfSeats)
            : base(manufacturer, model, year, startingBid, currentBid, VehicleType.Truck)
        {
            NumberOfSeats = numberOfSeats;
        }
    }
}
