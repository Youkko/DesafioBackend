namespace MotorcycleRental.Models.DTO
{
    public class ReturnUserParams
    {
        public Guid UserId { get; set; }
        public string? VIN { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}