namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class VINNotFoundException : Exception
    {
        private static readonly string DefaultMessage = "The specified VIN was not found in system!";

        public VINNotFoundException() : base(DefaultMessage) { }

        public VINNotFoundException(string message) : base(message) { }

        public VINNotFoundException(string message, Exception inner) : base(message, inner) { }

        public VINNotFoundException(Exception inner) : base(DefaultMessage, inner) { }
    }
}