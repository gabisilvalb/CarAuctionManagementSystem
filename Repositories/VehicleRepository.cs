using CarAuctionManagement.Models.Vehicles;
using CarAuctionManagementSystem.Models.DTOs.Requests;
using static CarAuctionManagementSystem.Exceptions.CustomExceptions;

namespace CarAuctionManagementSystem.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly List<Vehicle> _vehicles = new();

        public void Delete(Guid id)
        {
            {
                var index = _vehicles.FindIndex(v => v.Id == id);

                if (index == -1)
                    throw new VehicleNotFoundException(id);

                _vehicles.RemoveAt(index);
            }
        }

        public Vehicle Update(Vehicle vehicle)
        {
            var index = _vehicles.FindIndex(v => v.Id == vehicle.Id);

            if (index == -1)
                throw new VehicleNotFoundException(vehicle.Id);

            _vehicles[index] = vehicle;

            return vehicle;
        }

        public Vehicle GetById(Guid id)
        {
            return _vehicles.FirstOrDefault(v => v.Id == id);
        }

        public Vehicle AddVehicle(Vehicle vehicle)
        {

            _vehicles.Add(vehicle);

            return vehicle;
        }

        public IEnumerable<Vehicle> SearchVehicles(VehicleSearchParams searchParams)
        {
            var query = _vehicles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchParams.Manufacturer))
                query = query.Where(v => v.Manufacturer.Contains(searchParams.Manufacturer, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(searchParams.Model))
                query = query.Where(v => v.Model.Contains(searchParams.Model, StringComparison.OrdinalIgnoreCase));

            if (searchParams.Type.HasValue)
                query = query.Where(v => v.Type == searchParams.Type.Value);

            if (searchParams.Year.HasValue)
                query = query.Where(v => v.Year.Equals(searchParams.Year.Value));

            if (searchParams.StartingBid.HasValue)
                query = query.Where(v => v.StartingBid.Equals(searchParams.StartingBid.Value));

            return query.ToList();
        }

        //public Vehicle? GetById(string id)
        //{
        //    _vehicles.TryGetValue(id, out var vehicle);
        //    return vehicle;
        //}

        //public IEnumerable<Vehicle> Search(Func<Vehicle, bool> predicate)
        //    => _vehicles.Values.Where(predicate);

        public IEnumerable<Vehicle> GetAllVehicles() => _vehicles;
    }
}
