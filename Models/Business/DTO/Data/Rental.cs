namespace MotorcycleRental.Models.DTO
{
    public class Rental : ModelBase
    {
        public Guid UserId { get; set; }
        public Guid VehicleId { get; set; }
        public Guid RentalPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public Rental() { }

        public Rental(Database.Rental rental)
        {
            Id = rental.Id;
            UserId = rental.UserId;
            VehicleId = rental.VehicleId;
            RentalPlanId = rental.RentalPlanId;
            StartDate = rental.StartDate;
            EndDate = rental.EndDate;
            ReturnDate = rental.ReturnDate;
            CreatedOn = rental.CreatedOn;
            ModifiedOn = rental.ModifiedOn;
        }
    }
}