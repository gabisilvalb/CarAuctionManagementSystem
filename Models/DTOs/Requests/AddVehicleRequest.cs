using CarAuctionManagement.Models.Enums;
using FluentValidation;

namespace CarAuctionManagementSystem.Models.DTOs.Requests
{
    public class AddVehicleRequest
    {
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public decimal StartingBid { get; set; }
        public VehicleType? Type { get; set; }

        // Optional depending on type
        public int? NumberOfDoors { get; set; }
        public int? NumberOfSeats { get; set; }
        public decimal? LoadCapacity { get; set; }
    }
    public class AddVehicleRequestValidator : AbstractValidator<AddVehicleRequest>
    {
        public AddVehicleRequestValidator()
        {

            RuleFor(x => x.Manufacturer)
                .NotEmpty();

            RuleFor(x => x.Model)
                .NotEmpty();


            RuleFor(x => x.Year)
                .NotEmpty()
                .InclusiveBetween(1886, DateTime.UtcNow.Year)
                .WithMessage("Vehicle year must be valid and not in the future.");


            When(x => x.Type.HasValue && x.Type == VehicleType.SUV, () =>
            {
                RuleFor(x => x.NumberOfSeats)
                    .NotNull()
                    .GreaterThan(0)
                    .When(x => x.NumberOfSeats.HasValue)
                    .WithMessage("Number of seats must be provided for SUVs.");

                RuleFor(x => x.LoadCapacity)
                    .Must(value => value == null || value == 0)
                    .When(x => x.LoadCapacity.HasValue)
                    .WithMessage("Load capacity is not applicable for SUVs.");

                RuleFor(x => x.NumberOfDoors)
                    .Must(value => value == null || value == 0)
                    .When(x => x.NumberOfDoors.HasValue)
                    .WithMessage("Number of doors is not applicable for SUVs.");
            });

            When(x => x.Type.HasValue && (x.Type == VehicleType.Sedan || x.Type == VehicleType.Hatchback), () =>
            {
                RuleFor(x => x.NumberOfDoors)
                    .NotNull()
                    .GreaterThan(0)
                    .When(x => x.NumberOfDoors.HasValue)
                    .WithMessage("Number of doors must be provided for sedans and hatchbacks.");

                RuleFor(x => x.LoadCapacity)
                    .Must(value => value == null || value == 0)
                    .When(x => x.LoadCapacity.HasValue)
                    .WithMessage("Load capacity is not applicable for sedans and hatchbacks.");

                RuleFor(x => x.NumberOfSeats)
                    .Must(value => value == null || value == 0)
                    .When(x => x.NumberOfSeats.HasValue)
                    .WithMessage("Number of seats is not applicable for sedans and hatchbacks.");
            });

            When(x => x.Type.HasValue && x.Type == VehicleType.Truck, () =>
            {
                RuleFor(x => x.LoadCapacity)
                    .NotNull()
                    .GreaterThan(0)
                    .When(x => x.LoadCapacity.HasValue)
                    .WithMessage("Trucks must have a valid load capacity.");

                RuleFor(x => x.NumberOfDoors)
                    .Must(value => value == null || value == 0)
                    .When(x => x.NumberOfDoors.HasValue)
                    .WithMessage("Number of doors is not applicable for trucks.");

                RuleFor(x => x.NumberOfSeats)
                    .Must(value => value == null || value == 0)
                    .When(x => x.NumberOfSeats.HasValue)
                    .WithMessage("Number of seats is not applicable for trucks.");
            });

        }
    }
}
