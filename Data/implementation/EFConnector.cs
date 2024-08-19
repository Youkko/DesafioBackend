using Microsoft.EntityFrameworkCore;
using DB = MotorcycleRental.Models.Database;
using DTO = MotorcycleRental.Models.DTO;
namespace MotorcycleRental.Data
{
    public class EFConnector : IDisposable, IConnector
    {
        private readonly DatabaseContext _context;

        public EFConnector(DatabaseContext context)
        {
            _context = context;
        }

        private DbSet<DB.Delivery>? Deliveries => _context.Delivery;
        private DbSet<DB.DeliveryPerson>? DeliveryPeople => _context.DeliveryPerson;
        private DbSet<DB.Motorcycle>? Motorcycles => _context.Motorcycle;
        private DbSet<DB.Order>? Orders => _context.Order;
        private DbSet<DB.OrderItem>? OrderItems => _context.OrderItem;
        private DbSet<DB.Rental>? Rentals => _context.Rental;
        private DbSet<DB.RentalPlan>? RentalPlans => _context.RentalPlan;
        private DbSet<DB.User>? Users => _context.User;
        private DbSet<DB.UserType>? UserTypes => _context.UserType;
        private DbSet<DB.CNHType>? CNHTypes => _context.CNHType;
        private DbSet<DB.Notification>? Notifications => _context.Notification;

        public void Migrate() => _context.Database.Migrate();

        public async Task<T> AddAsync<T>(T entity) where T : class
        {
            await _context.Set<T>().AddAsync(entity);
            await SaveChangesAsync();
            return entity;
        }

        public async Task<T?> GetByIdAsync<T>(Guid id) where T : class
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> UpdateAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
            await SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync<T>(Guid id) where T : class
        {
            var entity = await GetByIdAsync<T>(id);
            if (entity == null)
            {
                return false;
            }

            _context.Set<T>().Remove(entity);
            await SaveChangesAsync();
            return true;
        }

        public async Task<DB.Motorcycle> GetMotorcycleAsync(Guid id) =>
            await Motorcycles!.FirstOrDefaultAsync(m => m.Id == id)??new();

        public async Task<IEnumerable<DB.Motorcycle>> GetAllMotorcyclesAsync() =>
            await Motorcycles!.ToListAsync();

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Authenticate an user
        /// </summary>
        /// <param name="userLogin">Login data for authentication (email and password)</param>
        /// <returns>User data if successful login || null.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<DTO.User?> Authenticate(DTO.UserLogin userLogin)
        {
            if (string.IsNullOrEmpty(userLogin.Email) || string.IsNullOrEmpty(userLogin.Password))
            {
                throw new ArgumentNullException(nameof(userLogin));
            }

            var users = await GetAllAsync<DB.User>();

            var user = users.FirstOrDefault(u => !string.IsNullOrEmpty(u.Email) &&
                                                 !string.IsNullOrEmpty(userLogin.Email) &&
                                                 u.Email.ToLower() == userLogin.Email.ToLower());
            return
                user != null ?
                    PasswordHelper.VerifyPassword(userLogin.Password!, user.Password!) ?
                    new Models.DTO.User(user) :
                    null : 
                null;
        }

        /// <summary>
        /// List all existing vehicles in the system
        /// </summary>
        /// <returns>IEnumerable with results || null</returns>
        public async Task<IEnumerable<DTO.Motorcycle>?> ListVehicles()
        {
            var allVehicles = await GetAllAsync<DB.Motorcycle>();
            IEnumerable<DTO.Motorcycle>? results = null;
            if (allVehicles != null)
            {
                List<DTO.Motorcycle> result = new List<DTO.Motorcycle>();
                foreach (var vehicle in allVehicles)
                {
                    var moto = new DTO.Motorcycle(vehicle);
                    result.Add(moto);
                }
                results = result;
            }
            return results;
        }

        /// <summary>
        /// Find an existing vehicle in the system by its VIN
        /// </summary>
        /// <param name="VIN">VIN number (exact match, case-insensitive).</param>
        /// <returns>Motorcycle object || null</returns>
        public async Task<DTO.Motorcycle?> FindVehicleByVIN(string VIN)
        {
            var vehicle = await Motorcycles!
                .FirstOrDefaultAsync(m => m.VIN != null &&
                                          m.VIN.ToLower() == VIN.ToLower());
            
            return vehicle != null ? new DTO.Motorcycle(vehicle) : null;
        }

        /// <summary>
        /// Replace VIN information for an existing vehicle
        /// </summary>
        /// <param name="vinParams">VIN edition data (Existing VIN, New VIN)</param>
        /// <returns>Boolean wether edition was successful</returns>
        public async Task<bool> ReplaceVIN(DTO.VINEditionParams vinParams)
        {
            var vehicle = await Motorcycles!
                .Where(m => !string.IsNullOrEmpty(m.VIN) &&
                            !string.IsNullOrEmpty(vinParams.ExistingVIN) &&
                            m.VIN.ToLower() == vinParams.ExistingVIN.ToLower())
                .FirstOrDefaultAsync();

            if (vehicle != null)
            {
                vehicle.VIN = vinParams.NewVIN;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Create a new vehicle in system.
        /// </summary>
        /// <param name="vehicleData">Vehicle details</param>
        /// <returns>Motorcycle object || null</returns>
        public async Task<DTO.Motorcycle?> CreateVehicle(DTO.MotorcycleCreation vehicleData)
        {
            var newVehicle = await Motorcycles!.AddAsync(new()
            {
                Brand = vehicleData.Brand,
                Model = vehicleData.Model,
                VIN = vehicleData.VIN,
                Year = vehicleData.Year,
            });

            var results = await _context.SaveChangesAsync();

            return await FindVehicleByVIN(vehicleData.VIN!);
        }

        /// <summary>
        /// Add a notification to database
        /// </summary>
        /// <param name="message">The message to add to database</param>
        public async void Notify(string message)
        {
            await Notifications!.AddAsync(new() { Message = message });
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}