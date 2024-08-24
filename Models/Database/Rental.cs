using System.ComponentModel.DataAnnotations;

namespace MotorcycleRental.Models.Database
{
    public class Rental : ModelBase
    {
        [Required]
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        [Required]
        public Guid VehicleId { get; set; }
        public virtual Vehicle? Vehicle { get; set; }

        [Required]
        public Guid RentalPlanId { get; set; }
        public virtual RentalPlan? RentalPlan { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public DateTime? ReturnDate { get; set; }

    }
}