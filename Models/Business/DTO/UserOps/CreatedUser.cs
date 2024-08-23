namespace MotorcycleRental.Models.DTO
{
    public class CreatedUser : ModelBase
    {
        public string? Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Email { get; set; }
        public string? CNPJ { get; set; }
        public string? CNH { get; set; }
        public string? CNHType { get; set; }
        
        public CreatedUser() { }

        public CreatedUser(
            Database.User user,
            Database.DeliveryPerson person,
            Database.CNHType cnhType)
        {
            Id = user.Id;
            Name = user.Name;
            BirthDate = user.BirthDate;
            Email = user.Email;
            CreatedOn = user.CreatedOn;
            CNPJ = person.CNPJ;
            CNH = person.CNH;
            CNHType = cnhType.Type;
        }
    }
}