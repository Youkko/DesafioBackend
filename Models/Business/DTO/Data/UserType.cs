namespace MotorcycleRental.Models.DTO
{
    public class UserType: ModelBase
    {
        public string? Description { get; set; }

        public ICollection<User>? Users { get; set; }

        public UserType() { }

        public UserType(Database.UserType userType)
        {
            Id = userType.Id;
            Description = userType.Description;
            Users = null;
            CreatedOn = userType.CreatedOn;
            ModifiedOn = userType.ModifiedOn;
        }
    }
}
