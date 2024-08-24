namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class UserNotFoundException : Exception
    {
        private static readonly string DefaultMessage = "User not found!";

        public UserNotFoundException() : base(DefaultMessage) { }

        public UserNotFoundException(string message) : base(message) { }

        public UserNotFoundException(string message, Exception inner) : base(message, inner) { }

        public UserNotFoundException(Exception inner) : base(DefaultMessage, inner) { }
    }
}