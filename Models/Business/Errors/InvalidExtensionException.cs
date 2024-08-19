namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class InvalidExtensionException : Exception
    {
        private static readonly string DefaultMessage = "Only PNG and BMP image files are accepted.";

        public InvalidExtensionException() : base(DefaultMessage) { }

        public InvalidExtensionException(string message) : base(message) { }

        public InvalidExtensionException(string message, Exception inner) : base(message, inner) { }

        public InvalidExtensionException(Exception inner) : base(DefaultMessage, inner) { }
    }
}