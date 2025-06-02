using CarAuctionManagement.Models.Enums;
using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;
using CarAuctionManagementSystem.Repositories.Auctions;
using CarAuctionManagementSystem.Repositories.Vehicles;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAuctionManagementSystem.Services.Vehicles
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IAuctionRepository _auctionRepository;

        public VehicleService(IVehicleRepository repository, IAuctionRepository auctionRepository)
        {
            _vehicleRepository = repository;
            _auctionRepository = auctionRepository;
        }

        public async Task DeleteVehicleAsync(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id) ?? throw new VehicleNotFoundException(id);

            if (await _auctionRepository.ExistsForVehicleAsync(id))
                throw new VehicleHaveActiveAuctionException(id);

            await _vehicleRepository.DeleteAsync(id);
        }

        public async Task<VehicleResponse?> UpdateVehicleAsync(UpdateVehicleRequest? request)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(request!.Id) ?? throw new VehicleNotFoundException(request.Id);

            if (await _auctionRepository.ExistsForVehicleAsync(request.Id))
                throw new VehicleHaveActiveAuctionException(request.Id);

            if (vehicle.Type != request.Type)
                throw new CannotUpdateVehicleType();

            vehicle.Manufacturer = request.Manufacturer ?? vehicle.Manufacturer;
            vehicle.Model = request.Model ?? vehicle.Model;
            vehicle.Year = request.Year ?? vehicle.Year;
            vehicle.StartingBid = request.StartingBid ?? vehicle.StartingBid;

            // Specifics
            if (vehicle is Sedan sedan && request.NumberOfDoors.HasValue)
                sedan.NumberOfDoors = request.NumberOfDoors.Value;

            if (vehicle is Hatchback hatchback && request.NumberOfDoors.HasValue)
                hatchback.NumberOfDoors = request.NumberOfDoors.Value;

            if (vehicle is SUV suv && request.NumberOfSeats.HasValue)
                suv.NumberOfSeats = request.NumberOfSeats.Value;

            if (vehicle is Truck truck && request.LoadCapacity.HasValue)
                truck.LoadCapacity = request.LoadCapacity.Value;

            var update = await _vehicleRepository.UpdateAsync(vehicle);

            var response = new VehicleResponse
            {
                Id = update.Id,
                Manufacturer = update.Manufacturer,
                Model = update.Model,
                Year = update.Year,
                StartingBid = update.StartingBid,
                Type = update.Type,
                NumberOfDoors = update is Sedan s ? s.NumberOfDoors : update is Hatchback h ? h.NumberOfDoors : null,
                NumberOfSeats = update is SUV s2 ? s2.NumberOfSeats : null,
                LoadCapacity = update is Truck t ? t.LoadCapacity : null
            };

            return response;
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
                throw new NoVehiclesFoundException();

            return vehicle;
        }

        public async Task<Vehicle> AddVehicleAsync(AddVehicleRequest request)
        {

            switch (request.Type)
            {
                case VehicleType.Hatchback:
                    var hatchback = CreateHatchback(request);
                    var vehicleHatchback = await _vehicleRepository.AddVehicleAsync(hatchback);
                    return vehicleHatchback;

                case VehicleType.Sedan:
                    Sedan sedan = CreateSedan(request);
                    var vehicleSedan = await _vehicleRepository.AddVehicleAsync(sedan);
                    return vehicleSedan;

                case VehicleType.SUV:
                    var suv = CreateSuv(request);
                    var vehicleSUV = await _vehicleRepository.AddVehicleAsync(suv);
                    return vehicleSUV;

                case VehicleType.Truck:
                    var truck = CreateTruck(request);
                    var vehicleTruck = await _vehicleRepository.AddVehicleAsync(truck);
                    return vehicleTruck;

                default:
                    throw new InvalidVehicleTypeException();
            }
        }
        public async Task<IEnumerable<Vehicle>> SearchVehiclesAsync(VehicleSearchParams searchParams)
        {
            return await _vehicleRepository.SearchVehiclesAsync(searchParams);
        }

        private static Truck CreateTruck(AddVehicleRequest vehicleRequest)
        {
            Truck truck = new Truck(
                vehicleRequest.Manufacturer!,
                vehicleRequest.Model!,
                vehicleRequest.Year,
                vehicleRequest.StartingBid,
                vehicleRequest.LoadCapacity ?? 0
            );
            return truck;
        }

        private static SUV CreateSuv(AddVehicleRequest vehicleRequest)
        {
            SUV suv = new SUV(
                vehicleRequest.Manufacturer!,
                vehicleRequest.Model!,
                vehicleRequest.Year,
                vehicleRequest.StartingBid,
                vehicleRequest.NumberOfSeats ?? 0
            );
            return suv;
        }

        private static Sedan CreateSedan(AddVehicleRequest vehicleRequest)
        {
            Sedan sedan = new Sedan(
                vehicleRequest.Manufacturer!,
                vehicleRequest.Model!,
                vehicleRequest.Year,
                vehicleRequest.StartingBid,
                vehicleRequest.NumberOfDoors ?? 0
            );
            return sedan;
        }

        private static Hatchback CreateHatchback(AddVehicleRequest vehicleRequest)
        {
            Hatchback hatchback = new Hatchback(
                vehicleRequest.Manufacturer!,
                vehicleRequest.Model!,
                vehicleRequest.Year,
                vehicleRequest.StartingBid,
                vehicleRequest.NumberOfDoors ?? 0
            );
            return hatchback;
        }
    }
}
