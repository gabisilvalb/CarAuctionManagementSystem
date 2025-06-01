using CarAuctionManagement.Models.Enums;

namespace CarAuctionManagement.Models.Vehicles
{
    public class Vehicle
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public decimal? StartingBid { get; set; }
        public decimal? CurrentBid { get; set; }
        public VehicleType? Type { get; set; }
        public bool IsAuctionActive { get; set; }

        protected Vehicle(string manufacturer, string model, int year, decimal startingBid, decimal currentBid, VehicleType type)
        {
            Manufacturer = manufacturer;
            Model = model;
            Year = year;
            StartingBid = startingBid;
            CurrentBid = currentBid;
            Type = type;
            IsAuctionActive = false;
        }


        //public virtual void Validate()
        //{
        //    if (Id == null || Guid.Empty.Equals(Id))
        //    {
        //        throw new CustomExceptions.ValidationException("Vehicle id must be provided.");
        //    }

        //    if (string.IsNullOrEmpty(Manufacturer))
        //    {
        //        throw new CustomExceptions.ValidationException("Manufacturer must be provided.");
        //    }

        //    if (string.IsNullOrEmpty(Model))
        //    {
        //        throw new CustomExceptions.ValidationException("Model must be provided.");
        //    }

        //    if (StartingBid <= 0)
        //    {
        //        throw new CustomExceptions.ValidationException("Starting bid must be greater than 0.");
        //    }

        //    if (Year < 1886 || Year > DateTime.Now.Year)
        //    {
        //        throw new CustomExceptions.ValidationException("Year must be a valid.");
        //    }
        //}
    }
}
