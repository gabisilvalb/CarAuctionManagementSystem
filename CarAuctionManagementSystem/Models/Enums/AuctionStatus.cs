using System.Text.Json.Serialization;

namespace CarAuctionManagement.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AuctionStatus
    {
        NotStarted,
        Closed,
        OnGoing
    }
}
