using System.ComponentModel.DataAnnotations;

namespace MotorcycleRental.Models.Database
{
    public class ModelBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedOn { get; set; }
    }
}
