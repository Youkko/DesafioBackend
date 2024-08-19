using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
namespace DeliveryPersonService
{
    public interface IVehicleOps
    {
        /// <summary>
        /// List all existing motorcycles in the system
        /// </summary>
        /// <param name="VIN">Optional. Filter by VIN number (full or partial, case-insensitive).</param>
        /// <returns>IEnumerable with results, or null</returns>
        Task<IEnumerable<Motorcycle>?> ListMotorcycles(string? VIN);
        /// <summary>
        /// Create a new vehicle in system.
        /// </summary>
        /// <param name="vehicleData">Vehicle details</param>
        /// <returns>Motorcycle object || null</returns>
        Task<Motorcycle?> CreateVehicle(MotorcycleCreation vehicleData);
        /// <summary>
        /// Modify a VIN
        /// </summary>
        /// <param name="vinParams">VIN edition data (Existing VIN, New VIN)</param>
        /// <returns>Boolean wether edition was successful</returns>
        /// <exception cref="RequiredInformationMissingException"></exception>
        /// <exception cref="VehicleNotFoundException"></exception>
        /// <exception cref="VINInUseException"></exception>
        Task<bool> EditVIN(VINEditionParams vinParams);
        /// <summary>
        /// Add a notification to database
        /// </summary>
        /// <param name="message">The message to add to database</param>
        public void Notify(string message);
    }
}