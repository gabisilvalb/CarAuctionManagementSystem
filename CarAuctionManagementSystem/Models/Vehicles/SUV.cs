using CarAuctionManagement.Models.Enums;

namespace CarAuctionManagement.Models.Vehicles
{
    public class SUV : Vehicle
    {
        public int? NumberOfSeats { get; set; }

        public SUV(string manufacturer, string model, int year, decimal startingBid, int numberOfSeats)
            : base(manufacturer, model, year, startingBid, VehicleType.SUV)
        {
            NumberOfSeats = numberOfSeats;
        }
    }
}
