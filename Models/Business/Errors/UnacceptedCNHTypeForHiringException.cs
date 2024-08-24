namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class UnacceptedCNHTypeForHiringException : Exception
    {
        private static readonly string DefaultMessage = "Only CNH Type A is allowed to hire!";

        public UnacceptedCNHTypeForHiringException() : base(DefaultMessage) { }

        public UnacceptedCNHTypeForHiringException(string message) : base(message) { }

        public UnacceptedCNHTypeForHiringException(string message, Exception inner) : base(message, inner) { }

        public UnacceptedCNHTypeForHiringException(Exception inner) : base(DefaultMessage, inner) { }
    }
}