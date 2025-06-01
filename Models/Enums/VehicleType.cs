using System.Text.Json.Serialization;

namespace CarAuctionManagement.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VehicleType
    {
        Hatchback,
        Sedan,
        SUV,
        Truck
    }
}
