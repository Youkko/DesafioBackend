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
        /// Calculate the rental total bill based on return date
        /// </summary>
        /// <param name="data">Rental information (VIN, return date)</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        Response PreviewVehicleReturn(ReturnUserParams data);
        /// <summary>
        /// List user rentals
        /// </summary>
        /// <param name="data">User ID</param>
        /// <returns>APIResponse object with success status and any relevant data</returns>
        Response ListUserRentals(string data);
    }
}