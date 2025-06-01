using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;
using CarAuctionManagementSystem.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarAuctionManagementSystem.Handlers
{
    public class VehicleHandlers
    {
        public static async Task<IResult> DeleteVehicle(Guid id, IVehicleService vehicleService)
        {

            await vehicleService.DeleteVehicleAsync(id);
            return Results.NoContent();
        }

        public static async Task<IResult> UpdateVehicle([FromBody] UpdateVehicleRequest request, IVehicleService vehicleService,
                IValidator<UpdateVehicleRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var result = vehicleService.UpdateVehicleAsync(request);

            return Results.Ok(new { result });
        }

        public static async Task<IResult> GetVehicleById(Guid id, IVehicleService vehicleService)
        {
            var vehicle = await vehicleService.GetVehicleByIdAsync(id);

            var result = new VehicleResponse
            {
                Id = vehicle!.Id,
                Manufacturer = vehicle.Manufacturer,
                Model = vehicle.Model,
                Year = vehicle.Year,
                StartingBid = vehicle.StartingBid,
                Type = vehicle.Type,
                NumberOfDoors = (vehicle is Sedan s1) ? s1.NumberOfDoors : (vehicle is Hatchback h) ? h.NumberOfDoors : null,
                NumberOfSeats = (vehicle is SUV s2) ? s2.NumberOfSeats : null,
                LoadCapacity = (vehicle is Truck t) ? t.LoadCapacity : null
            };

            return Results.Ok(new { result });
        }
        public static async Task<IResult> SearchVehicles(
                [AsParameters] VehicleSearchParams searchParams,
                IVehicleService vehicleService)
        {
            var vehicles = await vehicleService.SearchVehiclesAsync(searchParams);

            var result = vehicles.Select(v => new VehicleResponse
            {
                Id = v.Id,
                Manufacturer = v.Manufacturer,
                Model = v.Model,
                Year = v.Year,
                StartingBid = v.StartingBid,
                Type = v.Type,
                NumberOfDoors = (v is Sedan s1) ? s1.NumberOfDoors : (v is Hatchback h) ? h.NumberOfDoors : null,
                NumberOfSeats = (v is SUV s2) ? s2.NumberOfSeats : null,
                LoadCapacity = (v is Truck t) ? t.LoadCapacity : null
            });

            return Results.Ok(new { result });
        }

        public static async Task<IResult> AddVehicle([FromBody] AddVehicleRequest vehicleRequest,
                IVehicleService vehicleService,
                IValidator<AddVehicleRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(vehicleRequest);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            var vehicle = await vehicleService.AddVehicleAsync(vehicleRequest);
            return Results.Created($"/vehicles/{vehicle.Id}", new { id = vehicle.Id });
        }

    }


}
