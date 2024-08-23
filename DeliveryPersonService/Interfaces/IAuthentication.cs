using Microsoft.IdentityModel.Tokens;
using MotorcycleRental.Models.DTO;
namespace DeliveryPersonService
{
    public interface IAuthentication
    {
        Task<Response> AuthenticateAsync(UserLogin userLogin);
        SecurityToken? GenerateToken(User user, out string jwtToken);
    }
}