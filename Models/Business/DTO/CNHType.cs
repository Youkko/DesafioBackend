namespace MotorcycleRental.Models.DTO
{
    public class CNHType : ModelBase
    {
        public string? Type { get; set; }

        public ICollection<DeliveryPerson>? DeliveryPersons { get; set; }
    }
}
