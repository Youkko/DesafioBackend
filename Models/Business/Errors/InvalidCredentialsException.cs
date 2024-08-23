namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class InvalidCredentialsException : Exception
    {
        private static readonly string DefaultMessage = "Invalid email and/or password.";

        public InvalidCredentialsException() : base(DefaultMessage) { }

        public InvalidCredentialsException(string message) : base(message) { }

        public InvalidCredentialsException(string message, Exception inner) : base(message, inner) { }

        public InvalidCredentialsException(Exception inner) : base(DefaultMessage, inner) { }
    }
}