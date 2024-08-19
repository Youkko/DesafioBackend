namespace MotorcycleRental.Models.DTO
{
    public class Delivery : ModelBase
    {
        public Guid DeliveryPersonId { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime DropoffDate { get; set; }
        public string? Status { get; set; }

        public virtual DeliveryPerson? DeliveryPerson { get; set; }
    }
}
