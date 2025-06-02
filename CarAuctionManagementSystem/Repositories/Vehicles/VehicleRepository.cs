using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using CarAuctionManagementSystem.Repositories.Vehicles;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

public class VehicleRepository : IVehicleRepository
{
    private readonly List<Vehicle> _vehicles = new();

    public Task DeleteAsync(Guid id)
    {
        var index = _vehicles.FindIndex(v => v.Id == id);

        if (index == -1)
            throw new VehicleNotFoundException(id);

        _vehicles.RemoveAt(index);
        return Task.CompletedTask;
    }

    public Task<Vehicle> UpdateAsync(Vehicle vehicle)
    {
        var index = _vehicles.FindIndex(v => v.Id == vehicle.Id);

        if (index == -1)
            throw new VehicleNotFoundException(vehicle.Id);

        _vehicles[index] = vehicle;

        return Task.FromResult(vehicle);
    }

    public Task<Vehicle?> GetByIdAsync(Guid id)
    {
        var vehicle = _vehicles.FirstOrDefault(v => v.Id == id);
        return Task.FromResult(vehicle);
    }

    public Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
    {
        _vehicles.Add(vehicle);
        return Task.FromResult(vehicle);
    }

    public Task<IEnumerable<Vehicle>> SearchVehiclesAsync(VehicleSearchParams searchParams)
    {
        var query = _vehicles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchParams.Manufacturer))
            query = query.Where(v => v.Manufacturer!.Contains(searchParams.Manufacturer, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(searchParams.Model))
            query = query.Where(v => v.Model!.Contains(searchParams.Model, StringComparison.OrdinalIgnoreCase));

        if (searchParams.Type.HasValue)
            query = query.Where(v => v.Type == searchParams.Type.Value);

        if (searchParams.Year.HasValue)
            query = query.Where(v => v.Year.Equals(searchParams.Year.Value));

        if (searchParams.StartingBid.HasValue)
            query = query.Where(v => v.StartingBid.Equals(searchParams.StartingBid.Value));

        return Task.FromResult(query.AsEnumerable());
    }

    public Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
    {
        return Task.FromResult(_vehicles.AsEnumerable());
    }
}
