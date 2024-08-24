namespace MotorcycleRental.Models.Database
{
    public class RentalPlan : ModelBase
    {
        public int Days { get; set; }
        public double Value { get; set; }

        public ICollection<Rental>? Rentals { get; set; }
    }
}
