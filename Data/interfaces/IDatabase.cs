using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
namespace MotorcycleRental.Data
{
    public interface IDatabase
    {
        void Migrate();
        /// <summary>
        /// Get user data
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User object or null</returns>
        User? GetUser(Guid userId);
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
        IEnumerable<Vehicle>? ListVehicles();
        /// <summary>
        /// Find an existing vehicle in the system by it's VIN
        /// </summary>
        /// <param name="data">VIN number (exact match, case-insensitive).</param>
        /// <returns>Vehicle object || null</returns>
        Task<Vehicle?> FindVehicleByVIN(SearchVehicleParams data);
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
        /// <returns>Vehicle object || null</returns>
        Task<Vehicle?> CreateVehicle(CreateVehicleParams data);
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
        /// <summary>
        /// Creates a new user and Delivery Person registry.
        /// </summary>
        /// <param name="data">User data</param>
        /// <returns></returns>
        /// <exception cref="ExistingCNHException"></exception>
        /// <exception cref="ExistingCNPJException"></exception>
        /// <exception cref="InvalidCNHTypeException"></exception>
        Task<CreatedUser> CreateUser(CreateUserParams data);

        /// <summary>
        /// List all existing rental plans
        /// </summary>
        /// <returns>List of RentalPlan objects</returns>
        IEnumerable<RentalPlan> ListRentalPlans();

        /// <summary>
        /// Hire a vehicle.
        /// </summary>
        /// <param name="data">Rental details</param>
        /// <returns>Rental data</returns>
        Task<Rental> HireVehicle(RentalParams data);

        /// <summary>
        /// List user's rentals
        /// </summary>
        /// <param name="data">User Id</param>
        /// <returns>Rental data</returns>
        ICollection<RentalInfo> ListUserRentals(Guid userId);
    }
}