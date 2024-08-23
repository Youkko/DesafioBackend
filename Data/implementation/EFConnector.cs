using Microsoft.EntityFrameworkCore;
using MotorcycleRental.Models.Errors;
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
        /// <param name="data">VIN number (exact match, case-insensitive).</param>
        /// <returns>Motorcycle object || null</returns>
        public async Task<DTO.Motorcycle?> FindVehicleByVIN(DTO.SearchVehicleParams data)
        {
            var vehicle = await Motorcycles!
                .FirstOrDefaultAsync(m => m.VIN != null &&
                                          m.VIN.ToLower() == data.VIN!.ToLower());
            
            return vehicle != null ? new DTO.Motorcycle(vehicle) : null;
        }

        /// <summary>
        /// Replace VIN information for an existing vehicle
        /// </summary>
        /// <param name="data">VIN edition data (Existing VIN, New VIN)</param>
        /// <returns>Boolean wether edition was successful</returns>
        public async Task<bool> ReplaceVIN(DTO.EditVehicleParams data)
        {
            var vehicle = await Motorcycles!
                .Where(m => !string.IsNullOrEmpty(m.VIN) &&
                            !string.IsNullOrEmpty(data.ExistingVIN) &&
                            m.VIN.ToLower() == data.ExistingVIN.ToLower())
                .FirstOrDefaultAsync();

            if (vehicle != null)
            {
                vehicle.VIN = data.NewVIN;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Create a new vehicle in system.
        /// </summary>
        /// <param name="data">Vehicle details</param>
        /// <returns>Motorcycle object || null</returns>
        public async Task<DTO.Motorcycle?> CreateVehicle(DTO.CreateVehicleParams data)
        {
            var newVehicle = await Motorcycles!.AddAsync(new()
            {
                Brand = data.Brand,
                Model = data.Model,
                VIN = data.VIN,
                Year = data.Year,
            });

            var results = await _context.SaveChangesAsync();

            return await FindVehicleByVIN(new() { VIN = data.VIN });
        }

        /// <summary>
        /// Delete an existing vehicle by it's VIN information IF it has no rentals
        /// </summary>
        /// <param name="data">VIN</param>
        /// <returns>Boolean wether deletion was successful</returns>
        /// <exception cref="VehicleHasRentalsException"></exception>
        public async Task<bool> DeleteVehicle(DTO.DeleteVehicleParams data)
        {
            var vehicle = await Motorcycles!
                .Where(m => !string.IsNullOrEmpty(m.VIN) &&
                            !string.IsNullOrEmpty(data.VIN) &&
                            m.VIN.ToLower() == data.VIN.ToLower())
                .FirstOrDefaultAsync();

            if (vehicle != null)
            {
                if ((vehicle.Rentals != null && vehicle.Rentals.Count == 0) ||
                    vehicle.Rentals == null)
                {
                    Motorcycles!.Remove(vehicle);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                    throw new VehicleHasRentalsException();
            }
            return false;
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

        /// <summary>
        /// Creates a new user and Delivery Person registry.
        /// </summary>
        /// <param name="data">User data</param>
        /// <returns></returns>
        /// <exception cref="ExistingCNHException"></exception>
        /// <exception cref="ExistingCNPJException"></exception>
        /// <exception cref="InvalidCNHTypeException"></exception>
        public async Task<DTO.CreatedUser> CreateUser(DTO.CreateUserParams data)
        {
            if (DeliveryPeople!.Any(d => d.CNH == data.CNH))
                throw new ExistingCNHException();

            if (DeliveryPeople!.Any(d => d.CNPJ == data.CNPJ))
                throw new ExistingCNPJException();

            var cnhType = CNHTypes!.FirstOrDefault(t => t.Type!.ToLower() == data.CNHType!.ToLower());

            if (cnhType == null)
                throw new InvalidCNHTypeException();

            var userType = UserTypes!.First(t => t.Description!.ToLower() == "user");

            var resultUsr = await Users!.AddAsync(new()
            {
                Name = data.Name,
                Email = data.Email,
                BirthDate = data.BirthDate.ToUniversalTime(),
                Password = PasswordHelper.HashPassword(data.Password!),
                Enabled = true,
                UserTypeId = userType.Id
            });

            var user = resultUsr.Entity;
            
            var resultDlv = await DeliveryPeople!.AddAsync(new()
            {
                CNH = data.CNH,
                CNHTypeId = cnhType.Id,
                CNPJ = data.CNPJ,
                UserId = user.Id,
            });

            var delivery = resultDlv.Entity;

            _context.SaveChanges();

            return new(user, delivery, cnhType);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}