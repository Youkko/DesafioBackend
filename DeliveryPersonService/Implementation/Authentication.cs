using MotorcycleRental.Data;
using Microsoft.IdentityModel.Tokens;
using MotorcycleRental.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MotorcycleRental.Models.Errors;
using System.Text.Json;
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

        public async Task<Response> AuthenticateAsync(UserLogin userLogin)
        {
            var user = await _database.Authenticate(userLogin);

            if (user == null || !user.IsValid)
                throw new InvalidCredentialsException();

            
            LoginResponse loginResponse = new(false, string.Empty);

            string jwt = string.Empty;
            var token = GenerateToken(user, out jwt);
            loginResponse.IsAuthenticated = user.IsValid;
            loginResponse.Token = jwt;

            return new(JsonSerializer.Serialize(loginResponse), true);
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