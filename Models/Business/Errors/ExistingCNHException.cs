namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class ExistingCNHException : Exception
    {
        private static readonly string DefaultMessage = "That CNH is already registered!";

        public ExistingCNHException() : base(DefaultMessage) { }

        public ExistingCNHException(string message) : base(message) { }

        public  ExistingCNHException(string message, Exception inner) : base(message, inner) { }

        public ExistingCNHException(Exception inner) : base(DefaultMessage, inner) { }
    }
}