namespace CarAuctionManagementSystem.Exceptions
{
    public static class CustomExceptions
    {
        public class VehicleNotFoundException(Guid? vehicleId) : Exception($"Vehicle with ID {vehicleId} not found.");
        public class NoVehiclesFoundException() : Exception("No vehicles found in the inventory.");
        public class NoAuctionsFoundException() : Exception("No auctions found.");
        public class CannotUpdateVehicleType() : Exception("Cannot update vehicle type.");
        public class AuctionAlreadyStartedException() : Exception("Auction has already started.");
        public class AuctionCantStartedException() : Exception("Auction can't start without a vehicle.");
        public class AuctionAlreadyClosedException() : Exception("Auction has already been closed.");
        public class AuctionNotActiveException(Guid? auctionId) : Exception($"No active auctions found with ID {auctionId}");
        public class AuctionWithoutBidsException() : Exception($"Cannot close auction without any bids.");
        public class AuctionDoestHaveVehicleException() : Exception("Auction does not have a vehicle.");
        public class AuctionNotFoundException(Guid? auctionId) : Exception($"Auction with ID {auctionId} not found");
        public class BidAmountTooLowException(decimal? bidAmount, decimal? highestBid) : Exception($"The bid amount {bidAmount} is lower or equal to the current highest bid {highestBid}.");
        public class BidAmountLowerThanStartingPriceException(decimal? bidAmount, decimal? startingPrice) : Exception($"The bid amount {bidAmount} is lower than the starting price {startingPrice}.");
        public class ValidationException(string? message) : Exception(message);
        public class AuctionSameIdException(Guid? vehicleId) : Exception($"An auction for the vehicle ID {vehicleId} already exists.");
        public class InvalidVehicleTypeException() : Exception("This type of vehicle is unknown.");
        public class BidderAlreadyExistsException(string? email) : Exception($"Bidder with email {email} already exists.");
        public class BidderNotFoundByIdException(Guid? id) : Exception($"A bidder with the ID {id} was not found.");
        public class BidderHasPlacedBidsException() : Exception($"Bidder cannot be deleted as they have placed bids.");
        public class VehicleHaveActiveAuctionException(Guid? vehicleId) : Exception($"Vehicle with ID {vehicleId} have an active auction, so cannot be deleted or updated.");
    }
}
