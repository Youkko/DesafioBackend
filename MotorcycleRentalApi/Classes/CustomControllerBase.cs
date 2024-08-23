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
            _connection = connection;
            _channel = channel;
        }

        protected IActionResult ProcessResponse<T>(Response? response)
        {
            return (response == null || !response.Success) ?
                StatusCode(400, response) :
                typeof(T) != typeof(LoginResponse) ?
                    Ok(JsonSerializer.Deserialize<T>(response!.Message!)) :
                    ProcessAuthentication(response);
        }

        private IActionResult ProcessAuthentication(Response? response)
        {
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(response!.Message!);

            bool pass = loginResponse != null &&
                loginResponse.IsAuthenticated.HasValue &&
                loginResponse.IsAuthenticated.Value;
            
            return pass ? Ok(loginResponse) : Unauthorized();
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
