namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class InvalidPlanException : Exception
    {
        private static readonly string DefaultMessage = "There's no plan with that amount of days.";

        public InvalidPlanException() : base(DefaultMessage) { }

        public InvalidPlanException(string message) : base(message) { }

        public InvalidPlanException(string message, Exception inner) : base(message, inner) { }

        public InvalidPlanException(Exception inner) : base(DefaultMessage, inner) { }
    }
}