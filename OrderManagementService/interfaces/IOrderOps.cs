using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
namespace OrderManagementService
{
    public interface IOrderOps
    {
        /// <summary>
        /// List all existing rental plans
        /// </summary>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        Response ListPlans();
        /// <summary>
        /// Hire a vehicle.
        /// </summary>
        /// <param name="data">Hire details</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        Task<Response> HireVehicle(RentalParams data);
        /// <summary>
        /// Return a vehicle
        /// </summary>
        /// <param name="data">Vehicle return details</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        Task<Response> PreviewVehicleReturn(ReturnParams data);
        /// <summary>
        /// List user rentals
        /// </summary>
        /// <param name="data">User ID</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        Task<Response> ListUserRentals(string data);
    }
}