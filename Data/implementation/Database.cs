using DatabaseConfig = MotorcycleRental.Models.Database.DatabaseConfig;
using Microsoft.Extensions.Options;
using MotorcycleRental.Models.DTO;
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
        /// <param name="VIN">VIN number (exact match, case-insensitive).</param>
        /// <returns>Motorcycle object || null</returns>
        public Task<Motorcycle?> FindVehicleByVIN(string VIN) =>
            _connector.FindVehicleByVIN(VIN);

        /// <summary>
        /// Replace VIN information for an existing vehicle
        /// </summary>
        /// <param name="vinParams">VIN edition data (Existing VIN, New VIN)</param>
        /// <returns>Boolean wether edition was successful</returns>
        public Task<bool> ReplaceVIN(VINEditionParams vinParams) =>
            _connector.ReplaceVIN(vinParams);

        /// <summary>
        /// Create a new vehicle in system.
        /// </summary>
        /// <param name="vehicleData">Vehicle details</param>
        /// <returns>Motorcycle object || null</returns>
        public Task<Motorcycle?> CreateVehicle(MotorcycleCreation vehicleData) =>
            _connector.CreateVehicle(vehicleData);
        
        /// <summary>
        /// Add a notification to database
        /// </summary>
        /// <param name="message">The message to add to database</param>
        public void Notify(string message) => _connector.Notify(message);
    }
}