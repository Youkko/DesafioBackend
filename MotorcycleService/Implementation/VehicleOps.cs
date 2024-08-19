using MotorcycleRental.Data;
using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
namespace DeliveryPersonService
{
    public class VehicleOps : IVehicleOps
    {
        private readonly IConfiguration _configuration;
        private readonly IDatabase _database;
        public VehicleOps(
            IConfiguration configuration, 
            IDatabase database)
        {
            _configuration = configuration;
            _database = database;
        }

        /// <summary>
        /// List all existing motorcycles in the system
        /// </summary>
        /// <param name="VIN">Optional. Filter by VIN number (full or partial, case-insensitive).</param>
        /// <returns>IEnumerable with results, or null</returns>
        public async Task<IEnumerable<Motorcycle>?> ListMotorcycles(string? VIN)
        {
            var motorcycles = await _database.ListVehicles();
            return string.IsNullOrEmpty(VIN) ?
                motorcycles :
                motorcycles?
                    .Where(m => m.VIN!.Contains(VIN, StringComparison.InvariantCultureIgnoreCase))
                    .AsEnumerable();
        }

        /// <summary>
        /// Create a new vehicle in system.
        /// </summary>
        /// <param name="vehicleData">Vehicle details</param>
        /// <returns>Motorcycle object || null</returns>
        public async Task<Motorcycle?> CreateVehicle(MotorcycleCreation vehicleData)
        {
            if (string.IsNullOrEmpty(vehicleData.VIN) || 
                string.IsNullOrEmpty(vehicleData.Model) ||
                string.IsNullOrEmpty(vehicleData.Brand) ||
                !vehicleData.Year.HasValue)
                throw new RequiredInformationMissingException();

            var existingVIN = await _database.FindVehicleByVIN(vehicleData.VIN);

            if (existingVIN == null)
                throw new VINInUseException();

            var newVehicle = await _database.CreateVehicle(vehicleData);

            return newVehicle;
        }

        /// <summary>
        /// Modify a VIN
        /// </summary>
        /// <param name="vinParams">VIN edition data (Existing VIN, New VIN)</param>
        /// <returns>Boolean wether edition was successful</returns>
        /// <exception cref="RequiredInformationMissingException"></exception>
        /// <exception cref="VehicleNotFoundException"></exception>
        /// <exception cref="VINInUseException"></exception>
        public async Task<bool> EditVIN(VINEditionParams vinParams)
        {
            if (string.IsNullOrEmpty(vinParams.ExistingVIN) || string.IsNullOrEmpty(vinParams.NewVIN))
                throw new RequiredInformationMissingException();

            var existingVIN = await _database.FindVehicleByVIN(vinParams.ExistingVIN);
            var newVIN = await _database.FindVehicleByVIN(vinParams.NewVIN);

            if (existingVIN == null)
                throw new VehicleNotFoundException();

            if (newVIN != null)
                throw new VINInUseException();

            return await _database.ReplaceVIN(vinParams);
        }
    }
}