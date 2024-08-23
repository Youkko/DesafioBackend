namespace MotorcycleRental.Models.DTO
{
    public class Rental : ModelBase
    {
        public Guid MotorcycleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? RentalStatus { get; set; }

        public Rental() { }

        public Rental(Database.Rental rental)
        {
            Id = rental.Id;
            MotorcycleId = rental.MotorcycleId;
            StartDate = rental.StartDate;
            EndDate = rental.EndDate;
            RentalStatus = rental.RentalStatus;
            CreatedOn = rental.CreatedOn;
            ModifiedOn = rental.ModifiedOn;
        }
    }
}