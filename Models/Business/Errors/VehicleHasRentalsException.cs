namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class VehicleHasRentalsException : Exception
    {
        private static readonly string DefaultMessage = "The vehicle you're trying to remove has rentals registered!";

        public VehicleHasRentalsException() : base(DefaultMessage) { }

        public VehicleHasRentalsException(string message) : base(message) { }

        public VehicleHasRentalsException(string message, Exception inner) : base(message, inner) { }

        public VehicleHasRentalsException(Exception inner) : base(DefaultMessage, inner) { }
    }
}