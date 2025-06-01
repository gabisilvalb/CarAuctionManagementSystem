using CarAuctionManagement.Models.Enums;
using FluentValidation;

namespace CarAuctionManagementSystem.Models.DTOs.Requests
{
    public class AddVehicleRequest
    {
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal StartingBid { get; set; }
        public decimal CurrentBid { get; set; }
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

            When(x => x.Type.HasValue && x.Type == VehicleType.Sedan, () =>
            {
                RuleFor(x => x.NumberOfDoors)
                    .NotNull()
                    .When(x => x.NumberOfDoors.HasValue)
                    .WithMessage("Number of doors must be provided for sedans.");

                RuleFor(x => x.LoadCapacity)
                    .Must(value => value == null || value == 0)
                    .When(x => x.LoadCapacity.HasValue)
                    .WithMessage("Load capacity is not applicable for sedans.");

                RuleFor(x => x.NumberOfSeats)
                    .Must(value => value == null || value == 0)
                    .When(x => x.NumberOfSeats.HasValue)
                    .WithMessage("Number of seats is not applicable for sedans.");
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
