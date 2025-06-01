using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;

namespace CarAuctionManagementSystem.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<Vehicle> AddVehicle(AddVehicleRequest? request);
        Task<Vehicle?> GetVehicleById(Guid id);
        Task<IEnumerable<Vehicle>> SearchVehicles(VehicleSearchParams searchParams);
        Task<VehicleResponse?> UpdateVehicle(UpdateVehicleRequest request);
        Task DeleteVehicle(Guid Id);
    }
}
