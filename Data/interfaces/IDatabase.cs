using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
namespace MotorcycleRental.Data
{
    public interface IDatabase
    {
        void Migrate();
        /// <summary>
        /// Authenticate an user
        /// </summary>
        /// <param name="userLogin">Login data for authentication (email and password)</param>
        /// <returns>User data if successful login || null.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task<User?> Authenticate(UserLogin userLogin);
        /// <summary>
        /// List all existing vehicles in the system
        /// </summary>
        /// <returns>IEnumerable with results || null</returns>
        Task<IEnumerable<Motorcycle>?> ListVehicles();
        /// <summary>
        /// Find an existing vehicle in the system by it's VIN
        /// </summary>
        /// <param name="data">VIN number (exact match, case-insensitive).</param>
        /// <returns>Motorcycle object || null</returns>
        Task<Motorcycle?> FindVehicleByVIN(SearchVehicleParams data);
        /// <summary>
        /// Replace VIN information for an existing vehicle
        /// </summary>
        /// <param name="data">VIN edition data (Existing VIN, New VIN)</param>
        /// <returns>Boolean wether edition was successful</returns>
        Task<bool> ReplaceVIN(EditVehicleParams data);
        /// <summary>
        /// Create a new vehicle in system.
        /// </summary>
        /// <param name="data">Vehicle details</param>
        /// <returns>Motorcycle object || null</returns>
        Task<Motorcycle?> CreateVehicle(CreateVehicleParams data);
        /// <summary>
        /// Delete an existing vehicle by it's VIN information IF it has no rentals
        /// </summary>
        /// <param name="data">VIN</param>
        /// <returns>Boolean wether deletion was successful</returns>
        /// <exception cref="VehicleHasRentalsException"></exception>
        Task<bool> DeleteVehicle(DeleteVehicleParams data);
        /// <summary>
        /// Add a notification to database
        /// </summary>
        /// <param name="message">The message to add to database</param>
        void Notify(string message);
    }
}