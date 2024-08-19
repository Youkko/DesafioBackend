using System.ComponentModel.DataAnnotations;

namespace MotorcycleRental.Models.Database
{
    public class DeliveryPerson : ModelBase
    {
        [Required]
        public string? CNPJ { get; set; }

        [Required]
        public string? CNH { get; set; }

        [Required]
        public Guid CNHTypeId { get; set; }

        [Required]
        public string? CNHImage { get; set; }

        public CNHType? CNHType { get; set; }

        public virtual User? User { get; set; }

        public ICollection<Delivery>? Deliveries { get; set; }
    }
}
