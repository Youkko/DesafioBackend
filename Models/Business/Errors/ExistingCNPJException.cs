namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class ExistingCNPJException : Exception
    {
        private static readonly string DefaultMessage = "That CNPJ is already registered!";

        public ExistingCNPJException() : base(DefaultMessage) { }

        public ExistingCNPJException(string message) : base(message) { }

        public ExistingCNPJException(string message, Exception inner) : base(message, inner) { }

        public ExistingCNPJException(Exception inner) : base(DefaultMessage, inner) { }
    }
}