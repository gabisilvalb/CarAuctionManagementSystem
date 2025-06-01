using CarAuctionManagementSystem.Models.DTOs.Requests;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CarAuctionManagementSystem.Extensions
{
    public static class ValidationExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<AddVehicleRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateVehicleRequest>();
            services.AddValidatorsFromAssemblyContaining<StartAuctionRequest>();
            services.AddValidatorsFromAssemblyContaining<AddAuctionRequest>();
            services.AddValidatorsFromAssemblyContaining<AddVehiclesToAuctionRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<PlaceBidRequestValidator>();

            return services;
        }
    }
}
