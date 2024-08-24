using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
namespace MotorcycleService
{
    public interface IVehicleOps
    {
        /// <summary>
        /// List all existing motorcycles in the system
        /// </summary>
        /// <param name="data">Optional. Filter by VIN number (full or partial, case-insensitive).</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        Response ListVehicles(SearchVehicleParams data);
        /// <summary>
        /// Create a new vehicle in system.
        /// </summary>
        /// <param name="data">Vehicle details</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        Task<Response> CreateVehicle(CreateVehicleParams data);
        /// <summary>
        /// Modify a VIN
        /// </summary>
        /// <param name="data">VIN edition data (Existing VIN, New VIN)</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        /// <exception cref="RequiredInformationMissingException"></exception>
        /// <exception cref="VehicleNotFoundException"></exception>
        /// <exception cref="VINInUseException"></exception>
        Task<Response> EditVehicle(EditVehicleParams data);
        /// <summary>
        /// Delete an existing vehicle by it's VIN information IF it has no rentals
        /// </summary>
        /// <param name="data">Target vehicle's VIN</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        /// <exception cref="VehicleNotFoundException"></exception>
        /// <exception cref="VehicleHasRentalsException"></exception>
        Task<Response> DeleteVehicle(DeleteVehicleParams data);
        /// <summary>
        /// Add a notification to database
        /// </summary>
        /// <param name="message">The message to add to database</param>
        void Notify(string message);
    }
}