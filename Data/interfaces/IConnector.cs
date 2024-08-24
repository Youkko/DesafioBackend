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
        Task<DB.Vehicle> GetVehicleAsync(Guid id);
        Task<IEnumerable<DB.Vehicle>> GetAllVehiclesAsync();
        Task<int> SaveChangesAsync();
        /// <summary>
        /// Get user data
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User object or null</returns>
        DTO.User? GetUser(Guid userId);
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
        IEnumerable<DTO.Vehicle>? ListVehicles();
        /// <summary>
        /// Find an existing vehicle in the system by it's VIN
        /// </summary>
        /// <param name="data">VIN number (exact match, case-insensitive).</param>
        /// <returns>Vehicle object || null</returns>
        Task<DTO.Vehicle?> FindVehicleByVIN(DTO.SearchVehicleParams data);
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
        /// <returns>Vehicle object || null</returns>
        Task<DTO.Vehicle?> CreateVehicle(DTO.CreateVehicleParams data);
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
        /// <summary>
        /// Creates a new user and Delivery Person registry.
        /// </summary>
        /// <param name="data">User data</param>
        /// <returns></returns>
        /// <exception cref="ExistingCNHException"></exception>
        /// <exception cref="ExistingCNPJException"></exception>
        /// <exception cref="InvalidCNHTypeException"></exception>
        Task<DTO.CreatedUser> CreateUser(DTO.CreateUserParams data);
        /// <summary>
        /// List all existing rental plans
        /// </summary>
        /// <returns>List of RentalPlan objects</returns>
        IEnumerable<DTO.RentalPlan> ListRentalPlans();
        /// <summary>
        /// Hire a vehicle.
        /// </summary>
        /// <param name="data">Rental details</param>
        /// <returns>Rental data</returns>
        Task<DTO.Rental> HireVehicle(DTO.RentalParams data);
        /// <summary>
        /// List user's rentals
        /// </summary>
        /// <param name="data">User Id</param>
        /// <returns>Rental data</returns>
        ICollection<DTO.RentalInfo> ListUserRentals(Guid userId);
        void Dispose();
    }
}