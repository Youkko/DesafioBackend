using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
namespace DeliveryPersonService
{
    public interface IUserOps
    {
        /// <summary>
        /// Create a new user in system.
        /// </summary>
        /// <param name="data">User details</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        /// <exception cref="ExistingCNHException"></exception>
        /// <exception cref="ExistingCNPJException"></exception>
        /// <exception cref="InvalidCNHTypeException"></exception>
        Task<Response> CreateUser(CreateUserParams data);
    }
}