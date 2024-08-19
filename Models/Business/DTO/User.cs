namespace MotorcycleRental.Models.DTO
{
    public class User : ModelBase
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public bool? Enabled { get; set; }
        public Guid UserTypeId { get; set; }
        public virtual UserType? UserType { get; set; }
        public ICollection<DeliveryPerson>? DeliveryPerson { get; set; }
        public User(Database.User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;
            BirthDate = user.BirthDate;
            Enabled = user.Enabled;
            UserTypeId = user.UserTypeId;
            CreatedOn = user.CreatedOn;
            ModifiedOn = user.ModifiedOn;
        }
        public bool IsValid => Enabled.HasValue && Enabled.Value == true && !string.IsNullOrEmpty(Email);
    }
}