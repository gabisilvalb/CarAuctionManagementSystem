using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Extensions;
using CarAuctionManagementSystem.Handlers;
using CarAuctionManagementSystem.Models.DTOs.Responses;
using CarAuctionManagementSystem.Repositories;
using CarAuctionManagementSystem.Repositories.Auctions;
using CarAuctionManagementSystem.Services;
using CarAuctionManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => { options.ListenLocalhost(5001); });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Car Auction API", Version = "v1" });
    c.EnableAnnotations();
});

builder.Services.AddSingleton<IVehicleService, VehicleService>();
builder.Services.AddSingleton<IVehicleRepository, VehicleRepository>();
builder.Services.AddSingleton<IAuctionRepository, AuctionRepository>();
builder.Services.AddSingleton<IAuctionService, AuctionService>();

builder.Services.AddValidators();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Car Auction API v1"));


//app.UseMiddleware<ValidationExceptionMiddleware>();


app.UseHttpsRedirection();

// Vehicle endpoints
app.MapGet("/vehicles", VehicleHandlers.SearchVehicles)
    .WithName("SearchVehicles");

app.MapGet("/vehicles/{id:Guid}", VehicleHandlers.GetVehicleById)
    .WithName("GetVehicle")
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound);

app.MapPost("/vehicles", VehicleHandlers.AddVehicle)
    .WithName("AddVehicle")
    .Produces<Vehicle>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);

app.MapPut("/vehicles/update", VehicleHandlers.UpdateVehicle)
    .WithName("UpdateVehicle")
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound);

app.MapDelete("/vehicles/{id:Guid}", VehicleHandlers.DeleteVehicle)
    .WithName("DeleteVehicle")
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound);

// Auction endpoints
app.MapPost("/auctions", AuctionHandler.AddAuction)
    .WithName("AddAuction")
    .Produces<AuctionResponse>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);

app.MapPost("/auctions/start", AuctionHandler.StartAuction)
    .WithName("StartAuction")
    .Produces<StartAuctionResponse>()
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound);

app.MapGet("/auctions/all", AuctionHandler.GetAllAuctions)
    .WithName("GetAllAuctions")
    .Produces<GetAllAuctionsResponse>();

app.MapGet("/auctions/onGoing", AuctionHandler.GetAllOnGoingAuctions)
    .WithName("GetOnGoingAuctions")
    .Produces<GetAllOnGoingAuctionsResponse>();

app.MapPost("/auctions/bid", AuctionHandler.PlaceBid)
    .WithName("PlaceBid")
    .Produces<PlaceBidResponse>()
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound);


app.Run();