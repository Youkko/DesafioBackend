namespace MotorcycleRental.Models.DTO
{
    public class ReturnInfo
    {
        public DateTime StartDate { get; set; }
        public int RentalDays { get; set; }
        public double PlanValue { get; set; }
        public DateTime ReturnDate { get; set; }
        public int DaysDifference { get; set; }
        public double TotalBill { get; set; }
    }
}