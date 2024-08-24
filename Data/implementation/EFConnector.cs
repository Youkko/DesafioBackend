using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MotorcycleRental.Models.Errors;
using DB = MotorcycleRental.Models.Database;
using DTO = MotorcycleRental.Models.DTO;
namespace MotorcycleRental.Data
{
    public class EFConnector : IDisposable, IConnector
    {
        #region Object Instances
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public EFConnector(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #endregion

        #region Private DBSets
        private DbSet<DB.Delivery>? Deliveries => _context.Delivery;
        private DbSet<DB.DeliveryPerson>? DeliveryPeople => _context.DeliveryPerson;
        private DbSet<DB.Vehicle>? Vehicles => _context.Vehicle;
        private DbSet<DB.Order>? Orders => _context.Order;
        private DbSet<DB.OrderItem>? OrderItems => _context.OrderItem;
        private DbSet<DB.Rental>? Rentals => _context.Rental;
        private DbSet<DB.RentalPlan>? RentalPlans => _context.RentalPlan;
        private DbSet<DB.User>? Users => _context.User;
        private DbSet<DB.UserType>? UserTypes => _context.UserType;
        private DbSet<DB.CNHType>? CNHTypes => _context.CNHType;
        private DbSet<DB.Notification>? Notifications => _context.Notification;
        #endregion

        #region Default database operations
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

        public async Task<DB.Vehicle> GetVehicleAsync(Guid id) =>
            await Vehicles!.FirstOrDefaultAsync(m => m.Id == id)??new();

        public async Task<IEnumerable<DB.Vehicle>> GetAllVehiclesAsync() =>
            await Vehicles!.ToListAsync();

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        #endregion

        #region Interfaced methods
        /// <summary>
        /// Get user data
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User object or null</returns>
        public DTO.User? GetUser(Guid userId)
        {
            var user = Users!.Include(u => u.UserType)
                             .Include(u => u.DeliveryPerson)
                                 .ThenInclude(dp => dp.CNHType)
                             .Include(u => u.Rentals)
                             .FirstOrDefault(u => u.Id == userId);
            return _mapper.Map<DTO.User>(user);
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
            if (user == null)
                return null;

            var userType = UserTypes!.FirstOrDefault(ut => ut.Id == user.UserTypeId);
            var deliveryPerson = DeliveryPeople!.FirstOrDefault(dp => dp.Id == user.DeliveryPersonId);

            return
                PasswordHelper.VerifyPassword(userLogin.Password!, user.Password!) ?
                new DTO.User(user, deliveryPerson, userType) :
                null;
        }

        /// <summary>
        /// List all existing vehicles in the system
        /// </summary>
        /// <returns>IEnumerable with results || null</returns>
        public IEnumerable<DTO.Vehicle>? ListVehicles()
        {
            var allVehicles = Vehicles!.Include(v => v.Rentals);
            return _mapper.Map<IEnumerable<DTO.Vehicle>>(allVehicles);
        }

        /// <summary>
        /// Find an existing vehicle in the system by its VIN
        /// </summary>
        /// <param name="data">VIN number (exact match, case-insensitive).</param>
        /// <returns>Vehicle object || null</returns>
        public async Task<DTO.Vehicle?> FindVehicleByVIN(DTO.SearchVehicleParams data)
        {
            var vehicle = await Vehicles!
                .FirstOrDefaultAsync(m => m.VIN != null &&
                                          m.VIN.ToLower() == data.VIN!.ToLower());
            
            return _mapper.Map<DTO.Vehicle>(vehicle);
        }

        /// <summary>
        /// Replace VIN information for an existing vehicle
        /// </summary>
        /// <param name="data">VIN edition data (Existing VIN, New VIN)</param>
        /// <returns>Boolean wether edition was successful</returns>
        public async Task<bool> ReplaceVIN(DTO.EditVehicleParams data)
        {
            var vehicle = await Vehicles!
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
        /// <returns>Vehicle object || null</returns>
        public async Task<DTO.Vehicle?> CreateVehicle(DTO.CreateVehicleParams data)
        {
            var newVehicle = await Vehicles!.AddAsync(new()
            {
                Brand = data.Brand,
                Model = data.Model,
                VIN = data.VIN,
                Year = data.Year,
            });

            await _context.SaveChangesAsync();

            return _mapper.Map<DTO.Vehicle?>(await FindVehicleByVIN(new() { VIN = data.VIN }));
        }

        /// <summary>
        /// Delete an existing vehicle by it's VIN information IF it has no rentals
        /// </summary>
        /// <param name="data">VIN</param>
        /// <returns>Boolean wether deletion was successful</returns>
        /// <exception cref="VehicleHasRentalsException"></exception>
        public async Task<bool> DeleteVehicle(DTO.DeleteVehicleParams data)
        {
            var vehicle = await Vehicles!
                .Where(m => !string.IsNullOrEmpty(m.VIN) &&
                            !string.IsNullOrEmpty(data.VIN) &&
                            m.VIN.ToLower() == data.VIN.ToLower())
                .FirstOrDefaultAsync();

            if (vehicle != null)
            {
                if ((vehicle.Rentals != null && vehicle.Rentals.Count == 0) ||
                    vehicle.Rentals == null)
                {
                    Vehicles!.Remove(vehicle);
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

            user.DeliveryPersonId = delivery.Id;

            _context.SaveChanges();

            return new(user, delivery, cnhType);
        }

        /// <summary>
        /// List all existing rental plans
        /// </summary>
        /// <returns>List of RentalPlan objects</returns>
        public IEnumerable<DTO.RentalPlan> ListRentalPlans() =>
            RentalPlans!.OrderBy(p => p.Days)
                        .Select(p => _mapper.Map<DTO.RentalPlan>(p))
                        .AsEnumerable();

        /// <summary>
        /// Hire a vehicle.
        /// </summary>
        /// <param name="data">Rental details</param>
        /// <returns>Rental data</returns>
        public async Task<DTO.Rental> HireVehicle(DTO.RentalParams data)
        {
            var user = Users!.Where(u => u.Id.Equals(data.UserId)).FirstOrDefault();
            if (user == null)
                throw new UserNotFoundException();

            var vehicle = await Vehicles!
                .FirstOrDefaultAsync(m => m.VIN != null &&
                                          m.VIN.ToLower() == data.VIN!.ToLower());
            if (vehicle == null)
                throw new VehicleNotFoundException();

            if (vehicle.Rentals != null && vehicle.Rentals.Any(r => !r.ReturnDate.HasValue))
                throw new VehicleUnavailableException();

            var plan = RentalPlans!.Where(rp => rp.Days == data.PlanDays).FirstOrDefault();
            if (plan == null)
                throw new InvalidPlanException();

            var startDate = DateTime.Now.AddDays(1);
            var endDate = startDate.AddDays(plan.Days);

            var resultRnt = await Rentals!.AddAsync(new()
            {
                UserId = user.Id,
                VehicleId = vehicle.Id,
                RentalPlanId = plan.Id,
                StartDate = startDate.ToUniversalTime(),
                EndDate = endDate.ToUniversalTime(),
                ReturnDate = null,
            });

            _context.SaveChanges();

            return _mapper.Map<DTO.Rental>(resultRnt.Entity);
        }

        /// <summary>
        /// List user's rentals
        /// </summary>
        /// <param name="data">User Id</param>
        /// <returns>Rental data</returns>
        public ICollection<DTO.RentalInfo> ListUserRentals(Guid userId)
        {
            var user = GetUser(userId);
            if (user == null)
                throw new UserNotFoundException();

            var rentals = Users!.Include(u => u.Rentals!)
                                .ThenInclude(r => r.Vehicle!)
                                .Include(u => u.Rentals!)
                                .ThenInclude(r => r.RentalPlan)
                                .FirstOrDefault(u => u.Id == userId)?
                                .Rentals;

            var data = _mapper.Map<ICollection<DTO.RentalInfo>>(rentals);
            return data;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
        #endregion
    }
}