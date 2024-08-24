namespace MotorcycleRental.Models.DTO
{
    public class RentalInfo
    {
        public VehicleSimplified? Vehicle { get; set; }
        public RentalPlan? Plan { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status
        {
            get
            {
                return ReturnDate.HasValue ?
                    "Finished" :
                    "Active";
            }
        }
    }
}