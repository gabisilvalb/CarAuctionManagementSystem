using CarAuctionManagement.Models.Auction;
using CarAuctionManagement.Models.Enums;
using CarAuctionManagementSystem.Models.Auction;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;
using CarAuctionManagementSystem.Repositories.Auctions;
using CarAuctionManagementSystem.Repositories.Bidders;
using CarAuctionManagementSystem.Repositories.Vehicles;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAuctionManagementSystem.Services.Auctions
{
    public class AuctionService : IAuctionService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IAuctionRepository _auctionRepository;
        private readonly IBidderRepository _bidderRepository;

        public AuctionService(IVehicleRepository vehicleRepository, IAuctionRepository auctionRepository, IBidderRepository bidderRepository)
        {
            _vehicleRepository = vehicleRepository;
            _auctionRepository = auctionRepository;
            _bidderRepository = bidderRepository;
        }
        public async Task<AuctionBidsResponse> GetAuctionBidsAsync(Guid auctionId)
        {
            var auction = await _auctionRepository.GetByIdAsync(auctionId)
                ?? throw new AuctionNotFoundException(auctionId);

            var bids = auction.Bids?
                .Where(b => b != null)
                .Select(b => new BidDto
                {
                    BidId = b!.Id!.Value,
                    Bidder = b.Bidder,
                    Amount = b.Amount!.Value
                })
                .ToList() ?? new List<BidDto>();

            return new AuctionBidsResponse
            {
                AuctionId = auction.Id,
                Bids = bids
            };
        }

        public async Task<AuctionResponse> GetAuctionByIdAsync(Guid id)
        {
            var auction = await _auctionRepository.GetByIdAsync(id)
                ?? throw new AuctionNotFoundException(id);

            return new AuctionResponse
            {
                Id = auction.Id,
                Status = auction.Status ?? AuctionStatus.NotStarted,
                Vehicle = auction.Vehicle,
                HighestBid = auction.HighestBid,
                Bids = auction.Bids,
                HighestBidder = auction.HighestBidder
            };
        }

        public async Task<List<AuctionResponse>> GetAllClosedAuctionsAsync()
        {
            var all = await _auctionRepository.GetAllAsync();

            return all
                .Where(a => a.Status == AuctionStatus.Closed)
                .Select(a => new AuctionResponse
                {
                    Id = a.Id,
                    Status = a.Status!.Value,
                    Vehicle = a.Vehicle,
                    HighestBid = a.HighestBid,
                    Bids = a.Bids,
                    HighestBidder = a.HighestBidder,
                })
                .ToList();
        }

        public async Task<CloseAuctionResponse> CloseAuctionAsync(CloseAuctionRequest request)
        {
            var auction = await _auctionRepository.GetByIdAsync(request.AuctionId)
                ?? throw new AuctionNotFoundException(request.AuctionId);

            if (auction.Status != AuctionStatus.OnGoing)
                throw new AuctionNotActiveException(request.AuctionId);

            if (auction.Bids == null || !auction.Bids.Any())
                throw new AuctionWithoutBidsException();

            var closedAuction = await _auctionRepository.CloseAsync(auction.Id);

            return new CloseAuctionResponse
            {
                AuctionId = closedAuction.Id,
                Status = closedAuction.Status!.Value,
                FinalPrice = closedAuction.HighestBid,
                WinnerId = closedAuction.HighestBidder
            };
        }

        public async Task<PlaceBidResponse> PlaceBidAsync(PlaceBidRequest request)
        {
            var auction = await _auctionRepository.GetByIdAsync(request.AuctionId)
                ?? throw new AuctionNotFoundException(request.AuctionId);

            if (auction.Status != AuctionStatus.OnGoing)
                throw new AuctionNotActiveException(request.AuctionId);

            if (auction.Vehicle == null)
                throw new AuctionDoestHaveVehicleException();

            if (request.Amount <= auction.Vehicle.StartingBid)
                throw new BidAmountLowerThanStartingPriceException(request.Amount, auction.Vehicle.StartingBid);

            var bidder = await _bidderRepository.GetByIdAsync(request.BidderId) ?? throw new BidderNotFoundByIdException(request.BidderId);

            var currentHighest = auction.Bids?.Any() ?? false ? auction.Bids.Max(b => b?.Amount ?? 0) : 0;

            if (request.Amount <= currentHighest)
                throw new BidAmountTooLowException(request.Amount, currentHighest);

            var bid = new Bid(Guid.NewGuid(), auction.Id, request.Amount, bidder);

            var bidAdded = await _auctionRepository.PlaceBidAsync(auction.Id, bid);

            return new PlaceBidResponse
            {
                AuctionId = auction.Id,
                BidId = bidAdded.Id!.Value,
                Amount = bidAdded.Amount!.Value,
                BidderId = bidAdded.Bidder.Id,
                BidderName = bidAdded.Bidder.Name
            };
        }
        public async Task<List<AuctionResponse>> GetOnGoingAuctionsAsync()
        {
            var all = await _auctionRepository.GetAllAsync();

            return all
                .Where(a => a.Status == AuctionStatus.OnGoing)
                .Select(a => new AuctionResponse
                {
                    Id = a.Id,
                    Status = a.Status!.Value,
                    Vehicle = a.Vehicle,
                    HighestBid = a.HighestBid,
                    Bids = a.Bids,
                    HighestBidder = a.HighestBidder,
                })
                .ToList();
        }

        public async Task<List<AuctionResponse>> GetAllAuctionsAsync()
        {
            var auctions = await _auctionRepository.GetAllAsync();

            return auctions.Select(a => new AuctionResponse
            {
                Id = a.Id,
                Status = a.Status ?? AuctionStatus.NotStarted,
                Vehicle = a.Vehicle,
                Bids = a.Bids,
                HighestBid = a.HighestBid,
                HighestBidder = a.HighestBidder,
            }).ToList();
        }
        public async Task<StartAuctionResponse> StartAuctionAsync(Guid auctionId)
        {
            var auction = await _auctionRepository.GetByIdAsync(auctionId)
                            ?? throw new NoAuctionsFoundException();

            if (auction.Vehicle == null)
                throw new AuctionCantStartedException();

            if (auction.Status == AuctionStatus.OnGoing)
                throw new AuctionAlreadyStartedException();

            if (auction.Status == AuctionStatus.Closed)
                throw new AuctionAlreadyClosedException();

            var startedAuction = await _auctionRepository.StartAuctionAsync(auction);

            return new StartAuctionResponse
            {
                AuctionId = startedAuction.Id,
                Status = startedAuction.Status!.Value,
                StartedAt = DateTime.UtcNow
            };
        }

        public async Task<AuctionResponse> AddAuctionAsync(Guid vehicleId)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);

            if (vehicle == null)
                throw new VehicleNotFoundException(vehicleId);

            if (await _auctionRepository.ExistsForVehicleAsync(vehicleId))
                throw new AuctionSameIdException(vehicleId);

            var auction = new Auction(vehicle, AuctionStatus.NotStarted, new List<Bid?>(), null, null);
            await _auctionRepository.AddAsync(auction);

            return new AuctionResponse
            {
                Id = auction.Id,
                Vehicle = auction.Vehicle,
                Status = auction.Status ?? AuctionStatus.NotStarted,
                Bids = auction.Bids,
                HighestBid = auction.HighestBid,
                HighestBidder = auction.HighestBidder
            };

        }
    }
}
