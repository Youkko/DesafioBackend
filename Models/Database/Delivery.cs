using System.ComponentModel.DataAnnotations;

namespace MotorcycleRental.Models.Database
{
    public class Delivery : ModelBase
    {
        [Required]
        public Guid DeliveryPersonId { get; set; }
        public virtual DeliveryPerson? DeliveryPerson { get; set; }
        
        [Required]
        public DateTime PickupDate { get; set; }
        public DateTime? DropoffDate { get; set; }
        public string? Status { get; set; }
    }
}