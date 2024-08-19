namespace MotorcycleRental.Models.DTO
{
    public class DeliveryPerson : ModelBase
    {
        public string? CNPJ { get; set; }

        public string? CNH { get; set; }

        public Guid CNHTypeId { get; set; }

        public string? CNHImage { get; set; }

        public CNHType? CNHType { get; set; }

        public virtual User? User { get; set; }

        public ICollection<Delivery>? Deliveries { get; set; }
    }
}
