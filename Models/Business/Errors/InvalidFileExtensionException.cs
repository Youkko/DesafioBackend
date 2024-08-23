namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class InvalidFileExtensionException : Exception
    {
        private static readonly string DefaultMessage = "Only PNG and BMP image files are accepted.";

        public InvalidFileExtensionException() : base(DefaultMessage) { }

        public InvalidFileExtensionException(string message) : base(message) { }

        public InvalidFileExtensionException(string message, Exception inner) : base(message, inner) { }

        public InvalidFileExtensionException(Exception inner) : base(DefaultMessage, inner) { }
    }
}