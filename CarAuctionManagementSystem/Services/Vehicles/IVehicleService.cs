using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Models.DTOs.Responses;

namespace CarAuctionManagementSystem.Services.Vehicles
{
    public interface IVehicleService
    {
        Task<Vehicle> AddVehicleAsync(AddVehicleRequest request);
        Task<Vehicle?> GetVehicleByIdAsync(Guid id);
        Task<IEnumerable<Vehicle>> SearchVehiclesAsync(VehicleSearchParams searchParams);
        Task<VehicleResponse?> UpdateVehicleAsync(UpdateVehicleRequest request);
        Task DeleteVehicleAsync(Guid Id);
    }
}
