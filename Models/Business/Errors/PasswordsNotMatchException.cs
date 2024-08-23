namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class PasswordsNotMatchException : Exception
    {
        private static readonly string DefaultMessage = "The password does not match with confirmation!";

        public PasswordsNotMatchException() : base(DefaultMessage) { }

        public PasswordsNotMatchException(string message) : base(message) { }

        public PasswordsNotMatchException(string message, Exception inner) : base(message, inner) { }

        public PasswordsNotMatchException(Exception inner) : base(DefaultMessage, inner) { }
    }
}