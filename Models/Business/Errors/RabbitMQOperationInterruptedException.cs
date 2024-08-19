using RabbitMQ.Client.Exceptions;

namespace MotorcycleRental.Models.Errors
{
    [Serializable]
    public class RabbitMQOperationInterruptedException : OperationInterruptedException
    {
        private static readonly string DefaultMessage = "RabbitMQ operation interrupted";

        public RabbitMQOperationInterruptedException() : base(DefaultMessage) { }

        public RabbitMQOperationInterruptedException(string message) : base(message) { }

        public RabbitMQOperationInterruptedException(string message, Exception inner) : base(message, inner) { }

        public RabbitMQOperationInterruptedException(Exception inner) : base(DefaultMessage, inner) { }
    }
}