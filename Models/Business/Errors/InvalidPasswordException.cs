namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class InvalidPasswordException : Exception
    {
        private static readonly string DefaultMessage = "Invalid password!";

        public InvalidPasswordException() : base(DefaultMessage) { }

        public InvalidPasswordException(string message) : base(message) { }

        public InvalidPasswordException(string message, Exception inner) : base(message, inner) { }

        public InvalidPasswordException(Exception inner) : base(DefaultMessage, inner) { }
    }
}