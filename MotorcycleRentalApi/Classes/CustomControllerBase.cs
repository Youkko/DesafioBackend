using Microsoft.AspNetCore.Mvc;
using MotorcycleRental.Models.DTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MotorcycleRentalApi
{
    public class CustomControllerBase : ControllerBase
    {
        protected readonly IConnection _connection;
        protected readonly IModel _channel;

        protected string? correlationID;

        public CustomControllerBase(
            IConnection connection,
            IModel channel)
        {
            _connection = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER"),
                Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS")
            }.CreateConnection();

            _channel = _connection.CreateModel();
        }

        protected IActionResult ProcessResponse<T>(Response? response)
        {
            return (response == null || !response.Success) ?
                StatusCode(400, response) :
                Ok(JsonSerializer.Deserialize<T>(response!.Message!));
        }

        protected IActionResult ProcessResponse(Response? response)
        {
            return (response == null || !response.Success) ?
                StatusCode(400, response) :
                Ok(response.Success);
        }

        protected private void SendMessageAndListenForResponse(
            string message,
            string command,
            string sendTo,
            string replyTo,
            ref EventingBasicConsumer consumer)
        {
            correlationID = Guid.NewGuid().ToString();
            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationID;
            props.ReplyTo = replyTo;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("request", command);

            byte[] bytes = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                string.Empty,
                sendTo,
                props,
                bytes);

            _channel.BasicConsume(
                queue: replyTo,
                autoAck: false,
                consumer);

        }
    }
}
