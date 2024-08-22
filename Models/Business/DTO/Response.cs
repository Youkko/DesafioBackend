namespace MotorcycleRental.Models.DTO
{
    public class Response
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Response() { }
        public Response(string? message, bool success)
        {
            Message = message;
            Success = success;
        }
    }
}
