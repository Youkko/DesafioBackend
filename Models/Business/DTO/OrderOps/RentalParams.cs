namespace MotorcycleRental.Models.DTO
{
    public class RentalParams
    {
        public Guid UserId { get; set; }
        public string? VIN { get; set; }
        public int PlanDays { get; set; }

        public RentalParams() { }

        public RentalParams(Guid userId, string vin, int planDays)
        {
            UserId = userId;
            VIN = vin;
            PlanDays = planDays;
        }
    }
}