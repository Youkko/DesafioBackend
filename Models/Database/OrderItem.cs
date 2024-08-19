namespace MotorcycleRental.Models.Database
{
    public class OrderItem : ModelBase
    {
        public Guid OrderId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }

        public virtual Order? Order { get; set; }
    }
}
