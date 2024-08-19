using System.ComponentModel.DataAnnotations;

namespace MotorcycleRental.Models.Database
{
    public class Notification : ModelBase
    {
        [Required]
        public string? Message { get; set; }
    }
}