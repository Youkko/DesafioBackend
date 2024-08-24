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
        [Required]
        public string? Password { get; set; }
        public bool? Enabled { get; set; }

        public Guid UserTypeId { get; set; }
        public virtual UserType? UserType { get; set; }

        public Guid? DeliveryPersonId { get; set; }
        public virtual DeliveryPerson? DeliveryPerson { get; set; }

        public ICollection<Rental>? Rentals { get; set; }
    }
}
