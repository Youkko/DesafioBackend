using MotorcycleRental.Data;
using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
using System.Numerics;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace OrderManagementService
{
    public class OrderOps : IOrderOps
    {
        #region Object Instances
        private readonly ILogger<OrderOps> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDatabase _database;
        #endregion

        #region Constructor
        public OrderOps(
            ILogger<OrderOps> logger, 
            IConfiguration configuration,
            IDatabase database)
        {
            _logger = logger;
            _configuration = configuration;
            _database = database;
        }
        #endregion

        #region Private methods
        private string? FilterString(string? input, bool toLower = false)
        {
            if (string.IsNullOrEmpty(input)) return null;
            var pattern = @"[^A-Za-z0-9\-]";
            string result = Regex.Replace(input, pattern, string.Empty);
            if (input.ToLower() == "null") return null;
            return toLower?result.ToLower():result;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// List all existing rental plans
        /// </summary>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        public Response ListPlans()
        {
            Response response = new(JsonSerializer.Serialize(new List<RentalPlan>().AsEnumerable()), true);
            try
            {
                var plans = _database.ListRentalPlans();
                if (plans == null)
                    return response;

                response.Message = JsonSerializer.Serialize(plans);

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
        /// Hire a vehicle.
        /// </summary>
        /// <param name="data">Hire details</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        public async Task<Response> HireVehicle(RentalParams data)
        {
            Response response = new(null, true);
            try
            {
                var user = _database.GetUser(data.UserId);
                if (user == null)
                    throw new UserNotFoundException();

                if (user.DeliveryPerson == null ||
                    user.DeliveryPerson.CNHType == null)
                    throw new CompleteUserInfoRequiredForHiringException();

                if (!user.DeliveryPerson.CNHType.Type!.Contains("a", StringComparison.InvariantCultureIgnoreCase))
                    throw new UnacceptedCNHTypeForHiringException();

                var newHire = await _database.HireVehicle(data);
                response.Message = JsonSerializer.Serialize(newHire);
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
        /// List user rentals
        /// </summary>
        /// <param name="data">User ID</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        public async Task<Response> ListUserRentals(string data)
        {
            Response response = new(null, true);
            try
            {
                Guid userId = Guid.Parse(data);
                var rentals = _database.ListUserRentals(userId);
                response.Message = JsonSerializer.Serialize(rentals);
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
        /// Return a vehicle
        /// </summary>
        /// <param name="data">Vehicle return details</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        public async Task<Response> PreviewVehicleReturn(ReturnParams data)
        {
            Response response = new(null, true);
            try
            {
                //data.ExistingVIN = FilterString(data.ExistingVIN);
                //data.NewVIN = FilterString(data.NewVIN);
                //
                //if (string.IsNullOrEmpty(data.ExistingVIN) || 
                //    string.IsNullOrEmpty(data.NewVIN))
                //    throw new RequiredInformationMissingException();
                //
                //var existingVIN = await _database.FindVehicleByVIN(new() { VIN = data.ExistingVIN });
                //var newVIN = await _database.FindVehicleByVIN(new() { VIN = data.NewVIN });
                //
                //if (existingVIN == null)
                //    throw new VehicleNotFoundException();
                //
                //if (newVIN != null)
                //    throw new VINInUseException();
                //
                //bool success = await _database.ReplaceVIN(data);
                //
                //response.Success = success;
                //if (success)
                //{
                //    newVIN = await _database.FindVehicleByVIN(new() { VIN = data.NewVIN });
                //    response.Message = JsonSerializer.Serialize(newVIN);
                //}

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
        #endregion
    }
}