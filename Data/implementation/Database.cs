using DatabaseConfig = MotorcycleRental.Models.Database.DatabaseConfig;
using Microsoft.Extensions.Options;
using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
namespace MotorcycleRental.Data
{
    public class Database : IDatabase
    {
        private readonly IOptions<DatabaseConfig> _options;
        private readonly IConnector _connector;

        public Database(IOptions<DatabaseConfig> options)
        {
            _options = options;
            var curOptions = _options.Value;
            _connector = new EFConnector(new DatabaseContext(curOptions.ConnectionString!));
        }

        public void Migrate() => _connector.Migrate();

        /// <summary>
        /// Authenticate an user
        /// </summary>
        /// <param name="userLogin">Login data for authentication (email and password)</param>
        /// <returns>User data if successful login, else null.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<User?> Authenticate(UserLogin userLogin) => 
            _connector.Authenticate(userLogin);

        /// <summary>
        /// List all existing motorcycles in the system
        /// </summary>
        /// <returns>IEnumerable with results, or null</returns>
        public Task<IEnumerable<Motorcycle>?> ListVehicles() => 
            _connector.ListVehicles();

        /// <summary>
        /// Find an existing motorcycles in the system by it's VIN
        /// </summary>
        /// <param name="data">VIN number (exact match, case-insensitive).</param>
        /// <returns>Motorcycle object || null</returns>
        public Task<Motorcycle?> FindVehicleByVIN(SearchVehicleParams data) =>
            _connector.FindVehicleByVIN(data);

        /// <summary>
        /// Replace VIN information for an existing vehicle
        /// </summary>
        /// <param name="data">VIN edition data (Existing VIN, New VIN)</param>
        /// <returns>Boolean wether edition was successful</returns>
        public Task<bool> ReplaceVIN(EditVehicleParams data) =>
            _connector.ReplaceVIN(data);

        /// <summary>
        /// Create a new vehicle in system.
        /// </summary>
        /// <param name="data">Vehicle details</param>
        /// <returns>Motorcycle object || null</returns>
        public Task<Motorcycle?> CreateVehicle(CreateVehicleParams data) =>
            _connector.CreateVehicle(data);

        /// <summary>
        /// Delete an existing vehicle by it's VIN information IF it has no rentals
        /// </summary>
        /// <param name="data">VIN</param>
        /// <returns>Boolean wether deletion was successful</returns>
        /// <exception cref="VehicleHasRentalsException"></exception>
        public Task<bool> DeleteVehicle(DeleteVehicleParams data) =>
            _connector.DeleteVehicle(data);

        /// <summary>
        /// Add a notification to database
        /// </summary>
        /// <param name="message">The message to add to database</param>
        public void Notify(string message) => _connector.Notify(message);
    }
}