using MotorcycleRental.Data;
using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace DeliveryPersonService
{
    public class VehicleOps : IVehicleOps
    {
        private readonly ILogger<VehicleOps> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDatabase _database;
        public VehicleOps(
            ILogger<VehicleOps> logger, 
            IConfiguration configuration,
            IDatabase database)
        {
            _logger = logger;
            _configuration = configuration;
            _database = database;
        }

        private string? FilterString(string? input, bool toLower = false)
        {
            if (string.IsNullOrEmpty(input)) return null;
            var pattern = @"[^A-Za-z0-9\-]";
            string result = Regex.Replace(input, pattern, string.Empty);
            if (input.ToLower() == "null") return null;
            return toLower?result.ToLower():result;
        }

        /// <summary>
        /// List all existing vehicles
        /// </summary>
        /// <param name="data">Optional. Filter by VIN number (full or partial, case-insensitive).</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        public async Task<Response> ListVehicles(SearchVehicleParams data)
        {
            Response response = new(null, true);
            try
            {
                var vehicles = await _database.ListVehicles();
                if (vehicles == null)
                    return response;

                response.Message = JsonSerializer.Serialize(vehicles);

                string? targetVIN = FilterString(data.VIN!);
                if (!string.IsNullOrEmpty(targetVIN))
                {
                    var filtered = vehicles
                        .Where(m => m.VIN != null &&
                               m.VIN.Contains(targetVIN, StringComparison.InvariantCultureIgnoreCase));
                    response.Message = JsonSerializer.Serialize(filtered);
                }
                return response;
            }
            catch (AggregateException aEx)
            {
                response = new(string.Empty, false);
                aEx.Flatten().Handle(ex =>
                {
                    response.Message = ex.Message;
                    _logger.LogError(ex, ex.Message);
                    return true;
                });
                return response;
            }
            catch (Exception ex)
            {
                response = new(ex.Message, false);
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        /// <summary>
        /// Create a new vehicle in system.
        /// </summary>
        /// <param name="data">Vehicle details</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        public async Task<Response> CreateVehicle(CreateVehicleParams data)
        {
            Response response = new(null, true);
            try
            {
                data.VIN = FilterString(data.VIN);
                data.Model = FilterString(data.Model);
                data.Brand = FilterString(data.Brand);

                if (string.IsNullOrEmpty(data.VIN) ||
                    string.IsNullOrEmpty(data.Model) ||
                    string.IsNullOrEmpty(data.Brand) ||
                    !data.Year.HasValue)
                    throw new RequiredInformationMissingException();
                
                var existingVIN = await _database.FindVehicleByVIN(new() { VIN = data.VIN });
                if (existingVIN != null)
                    throw new VINInUseException();

                var newVehicle = await _database.CreateVehicle(data);
                response.Message = JsonSerializer.Serialize(newVehicle);
                return response;
            }
            catch (AggregateException aEx)
            {
                response = new(string.Empty, false);
                aEx.Flatten().Handle(ex =>
                {
                    response.Message = ex.Message;
                    _logger.LogError(ex, ex.Message);
                    return true;
                });
                return response;
            }
            catch (Exception ex)
            {
                response = new(ex.Message, false);
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        /// <summary>
        /// Modify a VIN
        /// </summary>
        /// <param name="data">VIN edition data (Existing VIN, New VIN)</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        /// <exception cref="RequiredInformationMissingException"></exception>
        /// <exception cref="VehicleNotFoundException"></exception>
        /// <exception cref="VINInUseException"></exception>
        public async Task<Response> EditVehicle(EditVehicleParams data)
        {
            Response response = new(null, true);
            try
            {
                data.ExistingVIN = FilterString(data.ExistingVIN);
                data.NewVIN = FilterString(data.NewVIN);

                if (string.IsNullOrEmpty(data.ExistingVIN) || 
                    string.IsNullOrEmpty(data.NewVIN))
                    throw new RequiredInformationMissingException();

                var existingVIN = await _database.FindVehicleByVIN(new() { VIN = data.ExistingVIN });
                var newVIN = await _database.FindVehicleByVIN(new() { VIN = data.NewVIN });

                if (existingVIN == null)
                    throw new VehicleNotFoundException();

                if (newVIN != null)
                    throw new VINInUseException();

                bool success = await _database.ReplaceVIN(data);

                response.Success = success;
                if (success)
                {
                    newVIN = await _database.FindVehicleByVIN(new() { VIN = data.NewVIN });
                    response.Message = JsonSerializer.Serialize(newVIN);
                }

                return response;
            }
            catch (AggregateException aEx)
            {
                response = new(string.Empty, false);
                aEx.Flatten().Handle(ex =>
                {
                    response.Message = ex.Message;
                    _logger.LogError(ex, ex.Message);
                    return true;
                });
                return response;
            }
            catch (Exception ex)
            {
                response = new(ex.Message, false);
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        /// <summary>
        /// Delete an existing vehicle by it's VIN information IF it has no rentals
        /// </summary>
        /// <param name="data">Target vehicle's VIN</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        /// <exception cref="VehicleNotFoundException"></exception>
        /// <exception cref="VehicleHasRentalsException"></exception>
        public async Task<Response> DeleteVehicle(DeleteVehicleParams data)
        {
            Response response = new(null, true);
            try
            {
                data.VIN = FilterString(data.VIN);

                if (string.IsNullOrEmpty(data.VIN))
                    throw new RequiredInformationMissingException();

                var existingVIN = await _database.FindVehicleByVIN(new() { VIN = data.VIN });

                if (existingVIN == null)
                    throw new VehicleNotFoundException();

                response.Success = await _database.DeleteVehicle(data);

                return response;
            }
            catch (AggregateException aEx)
            {
                response = new(string.Empty, false);
                aEx.Flatten().Handle(ex =>
                {
                    response.Message = ex.Message;
                    _logger.LogError(ex, ex.Message);
                    return true;
                });
                return response;
            }
            catch (Exception ex)
            {
                response = new(ex.Message, false);
                _logger.LogError(ex, ex.Message);
                return response;
            }
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