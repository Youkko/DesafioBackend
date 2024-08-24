namespace MotorcycleRental.Models.DTO
{
    public class RentalPlan : ModelBase
    {
        public int Days { get; set; }
        public double Value { get; set; }
        
        public RentalPlan() { }

        public RentalPlan(Database.RentalPlan rentalPlan)
        {
            Id = rentalPlan.Id;
            Days = rentalPlan.Days;
            Value = rentalPlan.Value;
            CreatedOn = rentalPlan.CreatedOn;
            ModifiedOn = rentalPlan.ModifiedOn;
        }
    }
}