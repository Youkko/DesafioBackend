namespace MotorcycleRental.Models.DTO
{
    public class CreateUserParams
    {
        // User
        public string? Name { get; set; }
        public DateTime BirthDate { get; set; }

        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }

        // DeliveryPerson
        public string? CNPJ { get; set; }   // Unique

        public string? CNH { get; set; }    // Unique

        public string? CNHType { get; set; } // A, B, AB
    }
}