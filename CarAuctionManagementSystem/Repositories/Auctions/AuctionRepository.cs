﻿using CarAuctionManagement.Models.Auction;
using CarAuctionManagement.Models.Enums;
using CarAuctionManagementSystem.Models.Auction;

namespace CarAuctionManagementSystem.Repositories.Auctions
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly List<Auction> _auctions = new();
        public Task<bool> HasBidsByBidderAsync(Guid bidderId)
        {
            var hasBids = _auctions
                .Where(a => a.Bids != null)
                .SelectMany(a => a.Bids!)
                .Any(b => b?.Bidder.Id == bidderId);

            return Task.FromResult(hasBids);
        }

        public Task<Auction> CloseAsync(Guid auctionId)
        {
            var auction = _auctions.FirstOrDefault(a => a.Id == auctionId);
            auction!.Status = AuctionStatus.Closed;

            return Task.FromResult(auction);
        }
        public Task<Bid> PlaceBidAsync(Guid auctionId, Bid bid)
        {
            var auction = _auctions.FirstOrDefault(a => a.Id == auctionId)
                ?? throw new KeyNotFoundException("Auction not found.");

            auction.Bids ??= new List<Bid?>();
            auction.Bids.Add(bid);

            auction.HighestBid = bid.Amount;
            auction.HighestBidder = bid.Bidder.Id;

            return Task.FromResult(bid);
        }
        public Task<List<Auction>> GetAllAsync()
        {
            return Task.FromResult(_auctions.ToList());
        }

        public Task<Auction> StartAuctionAsync(Auction auction)
        {
            auction.Status = AuctionStatus.OnGoing;
            return Task.FromResult(auction);
        }

        public Task<Auction?> GetByIdAsync(Guid id)
        {
            var auction = _auctions.FirstOrDefault(a => a.Id == id);
            return Task.FromResult(auction);
        }

        public Task AddAsync(Auction auction)
        {
            _auctions.Add(auction);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsForVehicleAsync(Guid vehicleId)
        {
            var exists = _auctions.Any(a => a.Vehicle?.Id == vehicleId);
            return Task.FromResult(exists);
        }
    }
}
