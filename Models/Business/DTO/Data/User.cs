namespace MotorcycleRental.Models.DTO
{
    public class User : ModelBase
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime BirthDate { get; set; }
        public bool? Enabled { get; set; }

        public Guid UserTypeId { get; set; }
        public virtual UserType? UserType { get; set; }

        public Guid? DeliveryPersonId { get; set; }
        public virtual DeliveryPerson? DeliveryPerson { get; set; }

        public ICollection<Rental>? Rentals { get; set; }

        public User() { }

        public User(
            Database.User user,
            Database.DeliveryPerson? deliveryPerson,
            Database.UserType? userType)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            BirthDate = user.BirthDate;
            Enabled = user.Enabled;
            UserTypeId = user.UserTypeId;
            UserType = userType == null ? null : new UserType(userType);
            DeliveryPersonId = user.DeliveryPersonId;
            DeliveryPerson = deliveryPerson == null ? null : new DeliveryPerson(deliveryPerson);
            Rentals = null;
            CreatedOn = user.CreatedOn;
            ModifiedOn = user.ModifiedOn;
        }
        public bool IsValid => Enabled.HasValue && Enabled.Value == true && !string.IsNullOrEmpty(Email);
    }
}