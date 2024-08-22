namespace MotorcycleRental.Models.DTO
{
    public class CreateVehicleParams
    {
        public string? VIN { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public string? Brand { get; set; }
    }
}