namespace MotorcycleRental.Models.Database
{
    public class Order : ModelBase
    {
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? Status { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
