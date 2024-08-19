namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class RequiredInformationMissingException : Exception
    {
        private static readonly string DefaultMessage = "Some required fields are missing!";

        public RequiredInformationMissingException() : base(DefaultMessage) { }

        public RequiredInformationMissingException(string message) : base(message) { }

        public RequiredInformationMissingException(string message, Exception inner) : base(message, inner) { }

        public RequiredInformationMissingException(Exception inner) : base(DefaultMessage, inner) { }
    }
}