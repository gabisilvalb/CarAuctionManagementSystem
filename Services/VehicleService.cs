using CarAuctionManagement.Models.Enums;
using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;
using CarAuctionManagementSystem.Repositories;
using CarAuctionManagementSystem.Services.Interfaces;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAuctionManagementSystem.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _repository;

        public VehicleService(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task DeleteVehicle(Guid id)
        {
            var vehicle = _repository.GetById(id) ?? throw new VehicleNotFoundException(id);

            _repository.Delete(id);
        }

        public async Task<VehicleResponse?> UpdateVehicle(UpdateVehicleRequest? request)
        {
            var vehicle = _repository.GetById(request.Id);
            if (vehicle == null)
                return null;

            vehicle.Manufacturer = request.Manufacturer ?? vehicle.Manufacturer;
            vehicle.Model = request.Model ?? vehicle.Model;
            vehicle.Year = request.Year ?? vehicle.Year;
            vehicle.StartingBid = request.StartingBid ?? vehicle.StartingBid;
            vehicle.CurrentBid = request.CurrentBid;

            // Specifics
            if (vehicle is Sedan sedan && request.NumberOfDoors.HasValue)
                sedan.NumberOfDoors = request.NumberOfDoors.Value;

            if (vehicle is Hatchback hatchback && request.NumberOfDoors.HasValue)
                hatchback.NumberOfDoors = request.NumberOfDoors.Value;

            if (vehicle is SUV suv && request.NumberOfSeats.HasValue)
                suv.NumberOfSeats = request.NumberOfSeats.Value;

            if (vehicle is Truck truck && request.LoadCapacity.HasValue)
                truck.LoadCapacity = request.LoadCapacity.Value;

            var update = _repository.Update(vehicle);

            var response = new VehicleResponse
            {
                Id = update.Id,
                Manufacturer = update.Manufacturer,
                Model = update.Model,
                Year = update.Year,
                StartingBid = update.StartingBid,
                CurrentBid = update.CurrentBid,
                IsAuctionActive = update.IsAuctionActive,
                Type = update.Type,
                NumberOfDoors = (update is Sedan s) ? s.NumberOfDoors : (update is Hatchback h) ? h.NumberOfDoors : null,
                NumberOfSeats = (update is SUV s2) ? s2.NumberOfSeats : null,
                LoadCapacity = (update is Truck t) ? t.LoadCapacity : null
            };

            return response;
        }

        public async Task<Vehicle?> GetVehicleById(Guid id)
        {
            try
            {

                var vehicle = _repository.GetById(id);
                if (vehicle == null)
                    throw new NoVehiclesFoundException();

                return vehicle;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Vehicle> AddVehicle(AddVehicleRequest? request)
        {

            switch (request.Type)
            {
                case VehicleType.Hatchback:
                    var hatchback = CreateHatchback(request);
                    var vehicleHatchback = _repository.AddVehicle(hatchback);
                    return vehicleHatchback;

                case VehicleType.Sedan:
                    Sedan sedan = CreateSedan(request);
                    var vehicleSedan = _repository.AddVehicle(sedan);
                    return vehicleSedan;

                case VehicleType.SUV:
                    var suv = CreateSuv(request);
                    var vehicleSUV = _repository.AddVehicle(suv);
                    return vehicleSUV;

                case VehicleType.Truck:
                    var truck = CreateTruck(request);
                    var vehicleTruck = _repository.AddVehicle(truck);
                    return vehicleTruck;

                default:
                    throw new ArgumentOutOfRangeException(nameof(request.Type), "Unsupported vehicle type");
            }
        }
        public async Task<IEnumerable<Vehicle>> SearchVehicles(VehicleSearchParams searchParams)
        {
            try
            {
                return _repository.SearchVehicles(searchParams);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static Truck CreateTruck(AddVehicleRequest vehicleRequest)
        {
            Truck truck = new Truck(
                vehicleRequest.Manufacturer,
                vehicleRequest.Model,
                vehicleRequest.Year,
                vehicleRequest.StartingBid,
                vehicleRequest.CurrentBid,
                vehicleRequest.LoadCapacity ?? 0
            );
            return truck;
        }

        private static SUV CreateSuv(AddVehicleRequest vehicleRequest)
        {
            SUV suv = new SUV(
                vehicleRequest.Manufacturer,
                vehicleRequest.Model,
                vehicleRequest.Year,
                vehicleRequest.StartingBid,
                vehicleRequest.CurrentBid,
                vehicleRequest.NumberOfSeats ?? 0
            );
            return suv;
        }

        private static Sedan CreateSedan(AddVehicleRequest vehicleRequest)
        {
            Sedan sedan = new Sedan(
                vehicleRequest.Manufacturer,
                vehicleRequest.Model,
                vehicleRequest.Year,
                vehicleRequest.StartingBid,
                vehicleRequest.CurrentBid,
                vehicleRequest.NumberOfDoors ?? 0
            );
            return sedan;
        }

        private static Hatchback CreateHatchback(AddVehicleRequest vehicleRequest)
        {
            Hatchback hatchback = new Hatchback(
                vehicleRequest.Manufacturer,
                vehicleRequest.Model,
                vehicleRequest.Year,
                vehicleRequest.StartingBid,
                vehicleRequest.CurrentBid,
                vehicleRequest.NumberOfDoors ?? 0
            );
            return hatchback;
        }
        //private void GetVehiclesValidation(List<Vehicle?>? vehicles)
        //{
        //    if (vehicles?.Count == 0)
        //        throw new CustomExceptions.NoVehiclesFoundException();
        //}
    }
}
