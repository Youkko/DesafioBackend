namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class ExistingVINException : Exception
    {
        private static readonly string DefaultMessage = "A motorcycle with that VIN already exists in system!";

        public ExistingVINException() : base(DefaultMessage) { }

        public ExistingVINException(string message) : base(message) { }

        public ExistingVINException(string message, Exception inner) : base(message, inner) { }

        public ExistingVINException(Exception inner) : base(DefaultMessage, inner) { }
    }
}