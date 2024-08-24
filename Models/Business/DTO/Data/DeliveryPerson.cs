namespace MotorcycleRental.Models.DTO
{
    public class DeliveryPerson : ModelBase
    {
        public string? CNPJ { get; set; }
        public string? CNH { get; set; }

        public Guid CNHTypeId { get; set; }
        public CNHType? CNHType { get; set; }

        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }

        public ICollection<Delivery>? Deliveries { get; set; }

        public DeliveryPerson() { }

        public DeliveryPerson(Database.DeliveryPerson deliveryPerson)
        {
            Id = deliveryPerson.Id;
            CNPJ = deliveryPerson.CNPJ;
            CNH = deliveryPerson.CNH;
            CNHTypeId = deliveryPerson.CNHTypeId;
            CNHType = deliveryPerson.CNHType != null ? new CNHType(deliveryPerson.CNHType) : null;
            UserId = deliveryPerson.UserId;
            User = null;
            Deliveries = null;
            CreatedOn = deliveryPerson.CreatedOn;
            ModifiedOn = deliveryPerson.ModifiedOn;
        }

    }
}
