namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class VehicleNotFoundException : Exception
    {
        private static readonly string DefaultMessage = "There's no motorcycle with that VIN in system!";

        public VehicleNotFoundException() : base(DefaultMessage) { }

        public VehicleNotFoundException(string message) : base(message) { }

        public VehicleNotFoundException(string message, Exception inner) : base(message, inner) { }

        public VehicleNotFoundException(Exception inner) : base(DefaultMessage, inner) { }
    }
}