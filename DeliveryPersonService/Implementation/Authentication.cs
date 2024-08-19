using MotorcycleRental.Data;
using Microsoft.IdentityModel.Tokens;
using MotorcycleRental.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace DeliveryPersonService
{
    public class Authentication : IAuthentication
    {
        private readonly IConfiguration _configuration;
        private readonly IDatabase _database;
        public Authentication(
            IConfiguration configuration, 
            IDatabase database)
        {
            _configuration = configuration;
            _database = database;
        }

        public async Task<LoginResponse?> AuthenticateAsync(UserLogin userLogin)
        {
            LoginResponse response = new(false, string.Empty);

            var user = await _database.Authenticate(userLogin);
            if (user == null)
            {
                return null;
            }

            if (user.IsValid)
            {
                string jwt = string.Empty;
                var token = GenerateToken(user, out jwt);
                response.IsAuthenticated = user.IsValid;
                response.Token = jwt;
            }

            return response;
        }

        public SecurityToken? GenerateToken(User user, out string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email!),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            jwtToken = tokenHandler.WriteToken(token);
            return token;
        }
    }
}