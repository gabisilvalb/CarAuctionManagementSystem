using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.DTOs.Requests;

namespace CarAuctionManagementSystem.Repositories.Vehicles
{
    public interface IVehicleRepository
    {
        Task DeleteAsync(Guid id);
        Task<Vehicle> UpdateAsync(Vehicle vehicle);
        Task<Vehicle?> GetByIdAsync(Guid id);
        Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
        Task<IEnumerable<Vehicle>> SearchVehiclesAsync(VehicleSearchParams searchParams);
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();

    }
}
