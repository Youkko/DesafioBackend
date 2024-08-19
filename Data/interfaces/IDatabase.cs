using MotorcycleRental.Models.DTO;
namespace MotorcycleRental.Data
{
    public interface IDatabase
    {
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
        /// <param name="VIN">VIN number (exact match, case-insensitive).</param>
        /// <returns>Motorcycle object || null</returns>
        Task<Motorcycle?> FindVehicleByVIN(string VIN);
        /// <summary>
        /// Replace VIN information for an existing vehicle
        /// </summary>
        /// <param name="vinParams">VIN edition data (Existing VIN, New VIN)</param>
        /// <returns>Boolean wether edition was successful</returns>
        Task<bool> ReplaceVIN(VINEditionParams vinParams);
        /// <summary>
        /// Create a new vehicle in system.
        /// </summary>
        /// <param name="vehicleData">Vehicle details</param>
        /// <returns>Motorcycle object || null</returns>
        Task<Motorcycle?> CreateVehicle(MotorcycleCreation vehicleData);
        /// <summary>
        /// Add a notification to database
        /// </summary>
        /// <param name="message">The message to add to database</param>
        void Notify(string message);
    }
}