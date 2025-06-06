﻿using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;
using CarAuctionManagementSystem.Services.Auctions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarAuctionManagementSystem.Handlers
{
    public class AuctionHandler
    {
        public static async Task<IResult> CloseAuction(CloseAuctionRequest request, IAuctionService service, IValidator<CloseAuctionRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            var result = await service.CloseAuctionAsync(request);
            return Results.Ok(new { result });
        }

        public static async Task<IResult> PlaceBid([FromBody] PlaceBidRequest request, IAuctionService service,
                IValidator<PlaceBidRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            var result = await service.PlaceBidAsync(request);
            return Results.Ok(new { result });
        }

        public static async Task<IResult> GetAllOnGoingAuctions(IAuctionService service)
        {
            var auctions = await service.GetOnGoingAuctionsAsync();

            return Results.Ok(new GetAllOnGoingAuctionsResponse
            {
                Auctions = auctions
            });
        }
        public static async Task<IResult> GetAllAuctions(IAuctionService service)
        {
            var auctions = await service.GetAllAuctionsAsync();

            return Results.Ok(new GetAllAuctionsResponse
            {
                Auctions = auctions
            });
        }
        public static async Task<IResult> AddAuction([FromBody] AddAuctionRequest auctionRequest,
                IAuctionService auctionService,
                IValidator<AddAuctionRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(auctionRequest);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            var auction = await auctionService.AddAuctionAsync(auctionRequest.VehicleId);
            return Results.Created($"/auctions/{auction.Id}", new { auction });
        }

        public static async Task<IResult> StartAuction([FromBody] StartAuctionRequest request, IAuctionService service, IValidator<StartAuctionRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            var result = await service.StartAuctionAsync(request.AuctionId);
            return Results.Ok(new { result });
        }

        public static async Task<IResult> GetAllClosedAuctions(IAuctionService service)
        {
            var auctions = await service.GetAllClosedAuctionsAsync();

            return Results.Ok(new GetAllClosedAuctionsResponse
            {
                Auctions = auctions
            });
        }

        public static async Task<IResult> GetAuctionById(Guid id, IAuctionService service)
        {
            var result = await service.GetAuctionByIdAsync(id);
            return Results.Ok(new { result });
        }

        public static async Task<IResult> GetAuctionBids(Guid id, IAuctionService service)
        {
            var result = await service.GetAuctionBidsAsync(id);
            return Results.Ok(new { result });
        }

    }
}
