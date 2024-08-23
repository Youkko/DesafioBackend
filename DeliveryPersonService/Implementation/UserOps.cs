using MotorcycleRental.Data;
using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace DeliveryPersonService
{
    public class UserOps : IUserOps
    {
        #region Object Instances
        private readonly ILogger<UserOps> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDatabase _database;
        #endregion

        #region Constructor
        public UserOps(
            ILogger<UserOps> logger, 
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
            var stripQuotesPattern = @"['""]";
            string result = Regex.Replace(input, stripQuotesPattern, string.Empty);
            if (input.ToLower() == "null") return null;
            return toLower?result.ToLower():result;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Create a new user in system.
        /// </summary>
        /// <param name="data">User details</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        public async Task<Response> CreateUser(CreateUserParams data)
        {
            Response response = new(null, true);
            try
            {
                data.Name = FilterString(data.Name);
                data.Email = FilterString(data.Email);
                data.CNPJ = FilterString(data.CNPJ);
                data.CNH = FilterString(data.CNH);
                data.CNHType = FilterString(data.CNHType);
                if (string.IsNullOrEmpty(data.Name) ||
                    string.IsNullOrEmpty(data.Email) ||
                    string.IsNullOrEmpty(data.CNPJ) ||
                    string.IsNullOrEmpty(data.CNH) ||
                    string.IsNullOrEmpty(data.CNHType))
                    throw new RequiredInformationMissingException();

                if (string.IsNullOrEmpty(data.Password))
                    throw new PasswordsNotMatchException();

                if (data.Password != data.ConfirmPassword!)
                    throw new InvalidPasswordException();

                var newUser = await _database.CreateUser(data);
                response.Message = JsonSerializer.Serialize(newUser);
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