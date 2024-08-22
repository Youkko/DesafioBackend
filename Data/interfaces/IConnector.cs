using DB = MotorcycleRental.Models.Database;
using DTO = MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
namespace MotorcycleRental.Data
{
    public interface IConnector
    {
        void Migrate();
        Task<T> AddAsync<T>(T entity) where T : class;
        Task<T?> GetByIdAsync<T>(Guid id) where T : class;
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
        Task<T> UpdateAsync<T>(T entity) where T : class;
        Task<bool> DeleteAsync<T>(Guid id) where T : class;
        Task<DB.Motorcycle> GetMotorcycleAsync(Guid id);
        Task<IEnumerable<DB.Motorcycle>> GetAllMotorcyclesAsync();
        Task<int> SaveChangesAsync();
        /// <summary>
        /// Authenticate an user
        /// </summary>
        /// <param name="userLogin">Login data for authentication (email and password)</param>
        /// <returns>User data if successful login || null.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task<DTO.User?> Authenticate(DTO.UserLogin userLogin);
        /// <summary>
        /// List all existing vehicles in the system
        /// </summary>
        /// <returns>IEnumerable with results || null</returns>
        Task<IEnumerable<DTO.Motorcycle>?> ListVehicles();
        /// <summary>
        /// Find an existing vehicle in the system by it's VIN
        /// </summary>
        /// <param name="data">VIN number (exact match, case-insensitive).</param>
        /// <returns>Motorcycle object || null</returns>
        Task<DTO.Motorcycle?> FindVehicleByVIN(DTO.SearchVehicleParams data);
        /// <summary>
        /// Replace VIN information for an existing vehicle
        /// </summary>
        /// <param name="data">VIN edition data (Existing VIN, New VIN)</param>
        /// <returns>Boolean wether edition was successful</returns>
        Task<bool> ReplaceVIN(DTO.EditVehicleParams data);
        /// <summary>
        /// Create a new vehicle in system.
        /// </summary>
        /// <param name="data">Vehicle details</param>
        /// <returns>Motorcycle object || null</returns>
        Task<DTO.Motorcycle?> CreateVehicle(DTO.CreateVehicleParams data);
        /// <summary>
        /// Delete an existing vehicle by it's VIN information IF it has no rentals
        /// </summary>
        /// <param name="data">VIN</param>
        /// <returns>Boolean wether deletion was successful</returns>
        /// <exception cref="VehicleHasRentalsException"></exception>
        Task<bool> DeleteVehicle(DTO.DeleteVehicleParams data);
        /// <summary>
        /// Add a notification to database
        /// </summary>
        /// <param name="message">The message to add to database</param>
        void Notify(string message);
        void Dispose();
    }
}