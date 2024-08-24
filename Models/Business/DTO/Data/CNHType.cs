namespace MotorcycleRental.Models.DTO
{
    public class CNHType : ModelBase
    {
        public string? Type { get; set; }

        public ICollection<DeliveryPerson>? DeliveryPersons { get; set; }

        public CNHType() { }

        public CNHType(Database.CNHType cnhType)
        {
            Id = cnhType.Id;
            Type = cnhType.Type;
            DeliveryPersons = null;
            CreatedOn = cnhType.CreatedOn;
            ModifiedOn = cnhType.ModifiedOn;
        }
    }
}
