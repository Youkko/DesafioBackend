namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class VINInUseException : Exception
    {
        private static readonly string DefaultMessage = "There specified VIN is already in use by an existing vehicle!";

        public VINInUseException() : base(DefaultMessage) { }

        public VINInUseException(string message) : base(message) { }

        public VINInUseException(string message, Exception inner) : base(message, inner) { }

        public VINInUseException(Exception inner) : base(DefaultMessage, inner) { }
    }
}