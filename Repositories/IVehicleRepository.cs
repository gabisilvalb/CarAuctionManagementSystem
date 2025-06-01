using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.DTOs.Requests;

namespace CarAuctionManagementSystem.Repositories
{
    public interface IVehicleRepository
    {
        Vehicle GetById(Guid id);
        Vehicle AddVehicle(Vehicle vehicle);
        IEnumerable<Vehicle> GetAllVehicles();
        IEnumerable<Vehicle> SearchVehicles(VehicleSearchParams searchParams);
        Vehicle Update(Vehicle vehicle);
        void Delete(Guid id);
    }
}
