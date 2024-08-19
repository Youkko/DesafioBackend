namespace MotorcycleRental.Models.Database
{
    public class Rental : ModelBase
    {
        public Guid MotorcycleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? RentalStatus { get; set; }

        public virtual Motorcycle? Motorcycle { get; set; }
    }
}
