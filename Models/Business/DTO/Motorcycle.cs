namespace MotorcycleRental.Models.DTO
{
    public class Motorcycle : ModelBase
    {
        public string? VIN { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public string? Brand { get; set; }

        //public ICollection<Rental>? Rentals { get; set; }
        public Motorcycle() { }

        public Motorcycle(Database.Motorcycle motorcycle)
        {
            Id = motorcycle.Id;
            VIN = motorcycle.VIN;
            Model = motorcycle.Model;
            Year = motorcycle.Year;
            Brand = motorcycle.Brand;
            CreatedOn = motorcycle.CreatedOn;
            ModifiedOn = motorcycle.ModifiedOn;
        }
    }
}