using CarAuctionManagement.Models.Enums;
using CarAuctionManagementSystem.Models.Bidder;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;
using CarAuctionManagementSystem.Repositories.Auctions;
using CarAuctionManagementSystem.Repositories.Bidders;
using CarAuctionManagementSystem.Services.Interfaces;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAuctionManagementSystem.Services
{
    public class BidderService : IBidderService
    {
        private readonly IBidderRepository _bidderRepository;
        private readonly IAuctionRepository _auctionRepository;

        public BidderService(IBidderRepository bidderRepository, IAuctionRepository auctionRepository)
        {
            _bidderRepository = bidderRepository;
            _auctionRepository = auctionRepository;
        }
        public async Task<BidderDetailsResponse> GetBidderWithBidsAsync(Guid bidderId)
        {
            var bidder = await _bidderRepository.GetByIdAsync(bidderId)
                ?? throw new BidderNotFoundByIdException(bidderId);

            var allAuctions = await _auctionRepository.GetAllAsync();

            var allBids = allAuctions
                .Where(a => a.Bids != null)
                .SelectMany(a => a.Bids!)
                .Where(b => b?.Bidder?.Id == bidderId)
                .Select(b => new BidHistoryDto
                {
                    AuctionId = b!.AuctionId,
                    Amount = b.Amount!.Value
                })
                .ToList();

            return new BidderDetailsResponse
            {
                Id = bidder.Id,
                Name = bidder.Name,
                Email = bidder.Email,
                Bids = allBids
            };
        }

        public async Task<BidderAuctionsResponse> GetAuctionsByBidderAsync(Guid bidderId)
        {
            var bidder = await _bidderRepository.GetByIdAsync(bidderId) ?? throw new BidderNotFoundByIdException(bidderId);

            var all = await _auctionRepository.GetAllAsync();

            var auctions = all
                .Where(a => a.Bids?.Any(b => b?.Bidder.Id == bidderId) == true)
                .Select(a => new BidderAuctionDto
                {
                    AuctionId = a.Id,
                    Status = a.Status ?? AuctionStatus.NotStarted,
                    VehicleId = a.Vehicle?.Id,
                    TotalBids = a.Bids!.Count(b => b?.Bidder.Id == bidderId),
                    LastBidAmount = a.Bids!
                        .Where(b => b?.Bidder.Id == bidderId)
                        .OrderByDescending(b => b?.Amount)
                        .FirstOrDefault()?.Amount
                })
                .ToList();

            return new BidderAuctionsResponse
            {
                BidderId = bidderId,
                Auctions = auctions
            };
        }
        public async Task DeleteBidderAsync(Guid id)
        {
            var bidder = await _bidderRepository.GetByIdAsync(id) ?? throw new BidderNotFoundByIdException(id);

            var hasBids = await _auctionRepository.HasBidsByBidderAsync(id);

            if (hasBids)
                throw new BidderHasPlacedBidsException();

            await _bidderRepository.DeleteAsync(id);
        }

        public async Task<CreateBidderResponse> CreateBidderAsync(CreateBidderRequest request)
        {
            var emailInUse = await _bidderRepository.EmailExistsAsync(request.Email!);
            if (emailInUse)
                throw new BidderAlreadyExistsException(request.Email);

            var bidder = new Bidder
            {
                Name = request.Name,
                Email = request.Email
            };

            var created = await _bidderRepository.AddAsync(bidder);

            return new CreateBidderResponse
            {
                Id = created.Id,
                Name = created.Name,
                Email = created.Email
            };
        }
    }
}
