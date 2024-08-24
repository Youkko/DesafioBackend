namespace MotorcycleRental.Models.DTO
{
    public class Vehicle : ModelBase
    {
        public string? VIN { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public string? Brand { get; set; }
        public ICollection<Rental>? Rentals { get; set; }
        
        public Vehicle() { }

        public Vehicle(Database.Vehicle vehicle)
        {
            Id = vehicle.Id;
            VIN = vehicle.VIN;
            Model = vehicle.Model;
            Year = vehicle.Year;
            Brand = vehicle.Brand;
            CreatedOn = vehicle.CreatedOn;
            ModifiedOn = vehicle.ModifiedOn;
            List<Rental> rentals = new List<Rental>();
            if (vehicle.Rentals != null &&
                vehicle.Rentals.Count > 0)
            {
                foreach (var rental in vehicle.Rentals)
                {
                    rentals.Add(new Rental(rental));
                }
            }
            Rentals = rentals;
        }
    }
}