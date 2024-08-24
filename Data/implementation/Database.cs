using DatabaseConfig = MotorcycleRental.Models.Database.DatabaseConfig;
using Microsoft.Extensions.Options;
using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
using AutoMapper;
namespace MotorcycleRental.Data
{
    public class Database : IDatabase
    {
        private readonly IOptions<DatabaseConfig> _options;
        private readonly IMapper _mapper;
        private readonly IConnector _connector;
        public Database(IOptions<DatabaseConfig> options, IMapper mapper)
        {
            _options = options;
            _mapper = mapper;
            var curOptions = _options.Value;
            _connector = new EFConnector(new DatabaseContext(curOptions.ConnectionString!), _mapper);
        }

        public void Migrate() => _connector.Migrate();

        /// <summary>
        /// Get user data
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User object or null</returns>
        public User? GetUser(Guid userId) => _connector.GetUser(userId);

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
        public IEnumerable<Vehicle>? ListVehicles() => 
            _connector.ListVehicles();

        /// <summary>
        /// Find an existing motorcycles in the system by it's VIN
        /// </summary>
        /// <param name="data">VIN number (exact match, case-insensitive).</param>
        /// <returns>Motorcycle object || null</returns>
        public Task<Vehicle?> FindVehicleByVIN(SearchVehicleParams data) =>
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
        public Task<Vehicle?> CreateVehicle(CreateVehicleParams data) =>
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

        /// <summary>
        /// Creates a new user and Delivery Person registry.
        /// </summary>
        /// <param name="data">User data</param>
        /// <returns></returns>
        /// <exception cref="ExistingCNHException"></exception>
        /// <exception cref="ExistingCNPJException"></exception>
        /// <exception cref="InvalidCNHTypeException"></exception>
        public Task<CreatedUser> CreateUser(CreateUserParams data) => 
            _connector.CreateUser(data);

        /// <summary>
        /// List all existing rental plans
        /// </summary>
        /// <returns>List of RentalPlan objects</returns>
        public IEnumerable<RentalPlan> ListRentalPlans() =>
            _connector.ListRentalPlans();

        /// <summary>
        /// Hire a vehicle.
        /// </summary>
        /// <param name="data">Rental details</param>
        /// <returns>Rental data</returns>
        public Task<Rental> HireVehicle(RentalParams data) =>
            _connector.HireVehicle(data);

        /// <summary>
        /// List user's rentals
        /// </summary>
        /// <param name="data">User Id</param>
        /// <returns>Rental data</returns>
        public ICollection<RentalInfo> ListUserRentals(Guid userId) =>
            _connector.ListUserRentals(userId);
    }
}