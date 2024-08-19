namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class InvalidCNHTypeException : Exception
    {
        private static readonly string DefaultMessage = "The specified CNH Type is invalid!";

        public InvalidCNHTypeException() : base(DefaultMessage) { }

        public InvalidCNHTypeException(string message) : base(message) { }

        public InvalidCNHTypeException(string message, Exception inner) : base(message, inner) { }

        public InvalidCNHTypeException(Exception inner) : base(DefaultMessage, inner) { }
    }
}