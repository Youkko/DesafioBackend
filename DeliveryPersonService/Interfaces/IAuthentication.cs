using Microsoft.IdentityModel.Tokens;
using MotorcycleRental.Models.DTO;
namespace DeliveryPersonService
{
    public interface IAuthentication
    {
        Task<LoginResponse?> AuthenticateAsync(UserLogin userLogin);
        SecurityToken? GenerateToken(User user, out string jwtToken);
    }
}