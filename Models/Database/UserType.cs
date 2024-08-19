namespace MotorcycleRental.Models.Database
{
    public class UserType : ModelBase
    {
        public string? Description { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
