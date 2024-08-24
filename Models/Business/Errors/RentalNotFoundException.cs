namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class RentalNotFoundException : Exception
    {
        private static readonly string DefaultMessage = "No active rental was found for the vehicle with that VIN for you!";

        public RentalNotFoundException() : base(DefaultMessage) { }

        public RentalNotFoundException(string message) : base(message) { }

        public RentalNotFoundException(string message, Exception inner) : base(message, inner) { }

        public RentalNotFoundException(Exception inner) : base(DefaultMessage, inner) { }
    }
}