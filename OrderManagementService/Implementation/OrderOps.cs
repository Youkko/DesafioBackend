using MotorcycleRental.Data;
using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
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
        public Response ListUserRentals(string data)
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
        /// Calculate the rental total bill based on return date
        /// </summary>
        /// <param name="data">Rental information (VIN, return date)</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        public Response PreviewVehicleReturn(ReturnUserParams data)
        {
            Response response = new(null, true);
            try
            {

                var rentals = _database.ListUserRentals(data.UserId);
                var targetRental = rentals
                    .Where(r => r.Status.Equals("active", StringComparison.InvariantCultureIgnoreCase) &&
                                r.Vehicle!.VIN!.Equals(data.VIN, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

                if (targetRental == null)
                    throw new RentalNotFoundException();

                /*
                 * As a delivery person, I want to inform the date on which I'll return the vehicle and want to query the rental bill.
                 * - When informed date is lower than estimated end date, the daily charge will be billed with an additional fee
                 *   - For the 7-day plan, the fee is 20% over each remaining day.
                 *   - For the 15-day plan, the fee is 40% over each remaining day.
                 * - When the informed date is higher than estemated end date, an additional fee of R$50,00 per exceeding day will be charged.
                 */

                ReturnInfo ri = new()
                {
                    StartDate = targetRental.StartDate,
                    RentalDays = targetRental.Plan!.Days,
                    PlanValue = targetRental.Plan!.Value,
                    ReturnDate = data.ReturnDate,
                    DaysDifference = (data.ReturnDate - targetRental.EndDate).Days,
                    TotalBill = 0
                };

                // Return before estimated end date
                if (ri.DaysDifference < 0) 
                {
                    int remainingDays = Math.Abs(ri.DaysDifference);
                    double feePercentage = 0;
                    if (ri.RentalDays == 7)
                        feePercentage = 0.2;
                    else if (ri.RentalDays == 15)
                        feePercentage = 0.4;

                    double dailyFee = ri.PlanValue * feePercentage;
                    ri.TotalBill = remainingDays * (ri.PlanValue + dailyFee);
                }
                // Return after estimated end date
                else if (ri.DaysDifference > 0) 
                {
                    double lateFee = 50.00;
                    ri.TotalBill = ri.DaysDifference * lateFee;
                }

                // Add the total bill for the actual rental period
                ri.TotalBill += ri.PlanValue;

                response.Message = JsonSerializer.Serialize(ri);
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