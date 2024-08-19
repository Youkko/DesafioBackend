using MotorcycleRental.Data;
using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
using System.Text.RegularExpressions;
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

        public string? FilterString(string? input, bool toLower = false)
        {
            if (string.IsNullOrEmpty(input)) return null;
            var pattern = @"[^A-Za-z0-9\-]";
            string result = Regex.Replace(input, pattern, string.Empty);
            if (input.ToLower() == "null") return null;
            return toLower?result.ToLower():result;
        }

        /// <summary>
        /// List all existing motorcycles in the system
        /// </summary>
        /// <param name="VIN">Optional. Filter by VIN number (full or partial, case-insensitive).</param>
        /// <returns>IEnumerable with results, or null</returns>
        public async Task<IEnumerable<Motorcycle>?> ListMotorcycles(string? VIN)
        {
            var motorcycles = await _database.ListVehicles();
            if (motorcycles == null)
            {
                return null;
            }
            string? targetVIN = FilterString(VIN!);
            if (!string.IsNullOrEmpty(targetVIN))
            {
                var filtered = motorcycles
                    .Where(m => m.VIN != null &&
                           m.VIN.Contains(targetVIN, StringComparison.InvariantCultureIgnoreCase));
                return filtered;
            }
            return motorcycles;
        }

        /// <summary>
        /// Create a new vehicle in system.
        /// </summary>
        /// <param name="vehicleData">Vehicle details</param>
        /// <returns>Motorcycle object || null</returns>
        public async Task<Motorcycle?> CreateVehicle(MotorcycleCreation vehicleData)
        {
            if (string.IsNullOrEmpty(FilterString(vehicleData.VIN)) || 
                string.IsNullOrEmpty(FilterString(vehicleData.Model)) ||
                string.IsNullOrEmpty(FilterString(vehicleData.Brand)) ||
                !vehicleData.Year.HasValue)
                throw new RequiredInformationMissingException();

            var existingVIN = await _database.FindVehicleByVIN(FilterString(vehicleData.VIN)!);

            if (existingVIN != null)
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

        /// <summary>
        /// Add a notification to database
        /// </summary>
        /// <param name="message">The message to add to database</param>
        public void Notify(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                _database.Notify(message);
            }
        }
    }
}