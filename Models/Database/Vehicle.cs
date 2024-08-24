using System.ComponentModel.DataAnnotations;

namespace MotorcycleRental.Models.Database
{
    public class Vehicle : ModelBase
    {
        [Required]
        public string? VIN { get; set; }
        [Required]
        public string? Model { get; set; }
        [Required]
        public int? Year { get; set; }
        public string? Brand { get; set; }

        public ICollection<Rental>? Rentals { get; set; }
    }
}
