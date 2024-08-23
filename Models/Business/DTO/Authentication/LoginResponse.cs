namespace MotorcycleRental.Models.DTO
{
    public class LoginResponse
    {
        public bool? IsAuthenticated { get; set; }
        public string? Token { get; set; }
        public LoginResponse(bool? isAuthenticated, string? token)
        {
            IsAuthenticated = isAuthenticated;
            Token = token;
        }
    }
}