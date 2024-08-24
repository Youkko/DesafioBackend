namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class VehicleUnavailableException : Exception
    {
        private static readonly string DefaultMessage = "The vehicle you're trying to hire is unavailable!";

        public VehicleUnavailableException() : base(DefaultMessage) { }

        public VehicleUnavailableException(string message) : base(message) { }

        public VehicleUnavailableException(string message, Exception inner) : base(message, inner) { }

        public VehicleUnavailableException(Exception inner) : base(DefaultMessage, inner) { }
    }
}