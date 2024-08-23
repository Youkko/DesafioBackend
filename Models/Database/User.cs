using System.ComponentModel.DataAnnotations;

namespace MotorcycleRental.Models.Database
{
    public class User : ModelBase
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }

        public string? Password { get; set; }
        public bool? Enabled { get; set; }

        public Guid UserTypeId { get; set; }
        public virtual UserType? UserType { get; set; }

        public ICollection<DeliveryPerson>? DeliveryPerson { get; set; }
    }
}
