namespace CarAuctionManagementSystem.Exceptions
{
    public abstract class CustomExceptions
    {
        public class VehicleNotFoundException(Guid? vehicleId) : Exception($"Vehicle with ID {vehicleId} not found.");
        public class VehicleAlreadyExistsException(Guid? vehicleId) : Exception($"Vehicle with ID {vehicleId} already exists on the inventory.");
        public class VehicleDataNullException() : Exception("Vehicle need to have data.");
        public class NoVehiclesFoundException() : Exception("No vehicles found in the inventory.");
        public class UnsupportedUpdateException() : Exception("Cannot change vehicle type once created.");
        public class NoVehiclesFoundWithFiltersException() : Exception("No vehicles with the specified filters found.");
        public class AuctionAlreadyActiveException(Guid? vehicleId) : Exception($"An auction is already active for vehicle ID {vehicleId}.");
        public class NoAuctionsFoundException() : Exception("No auctions found.");
        public class AuctionAlreadyStartedException() : Exception("Auction has already started.");
        public class AuctionHasAlreadyVehicleException() : Exception("Auction already has a vehicle assigned.");
        public class AuctionCantStartedException() : Exception("Auction can't start without a vehicle.");
        public class AuctionAlreadyClosedException() : Exception("Auction has already been closed.");
        public class NoClosedAuctionsFoundException() : Exception("No closed auctions found.");
        public class AuctionNotActiveException(Guid? auctionId) : Exception($"No active auctions found with ID {auctionId}");
        public class AuctionDoestHaveVehicleException() : Exception("Auction does not have a vehicle.");
        public class AuctionNotFoundException(Guid? auctionId) : Exception($"Auction with ID {auctionId} not found");
        public class BidAmountTooLowException(decimal? bidAmount, decimal? highestBid) : Exception($"The bid amount {bidAmount} is lower or equal to the current highest bid {highestBid}.");
        public class BidAmountLowerThanStartingPriceException(decimal? bidAmount, decimal? startingPrice) : Exception($"The bid amount {bidAmount} is lower than the starting price {startingPrice}.");
        public class ValidationException(string? message) : Exception(message);
        public class AuctionSameIdException(Guid? vehicleId) : Exception($"An auction for the vehicle ID {vehicleId} already exists.");
        public class InvalidVehicleTypeException() : Exception("This type of vehicle is unknown.");
        public class BidderAlreadyExistsException(string? getEmail, string? getName) : Exception($"A bidder with the email {getEmail} and name {getName} already exists.");
        public class BidderNotFoundByIdException(Guid? id) : Exception($"A bidder with the ID {id} was not found.");
        public class BidderNotFoundException() : Exception("No bidders found.");
        public class NoHighestBidderException(Guid? auctionId) : Exception($"No bidder found for auction with ID {auctionId}.");
        public class VehicleHaveActiveAuctionException(Guid? vehicleId, Guid? auctionId) : Exception($"Vehicle with ID {vehicleId} have an active auction with ID {auctionId}, so cannot be updated.");
    }
}
