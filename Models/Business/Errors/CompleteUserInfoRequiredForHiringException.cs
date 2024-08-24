namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class CompleteUserInfoRequiredForHiringException : Exception
    {
        private static readonly string DefaultMessage = "You cannot hire because your user information is incomplete!";

        public CompleteUserInfoRequiredForHiringException() : base(DefaultMessage) { }

        public CompleteUserInfoRequiredForHiringException(string message) : base(message) { }

        public CompleteUserInfoRequiredForHiringException(string message, Exception inner) : base(message, inner) { }

        public CompleteUserInfoRequiredForHiringException(Exception inner) : base(DefaultMessage, inner) { }
    }
}