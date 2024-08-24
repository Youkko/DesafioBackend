using Microsoft.AspNetCore.Mvc;
using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
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

        protected void SendMessageAndListenForResponse(
            string message,
            string command,
            string sendTo,
            string replyTo,
            ILogger logger,
            ref EventingBasicConsumer consumer,
            ref TaskCompletionSource<IActionResult> tcs)
        {
            correlationID = Guid.NewGuid().ToString();
            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationID;
            props.ReplyTo = replyTo;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("request", command);

            byte[] bytes = Encoding.UTF8.GetBytes(message);

            try
            {
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
            catch (RabbitMQOperationInterruptedException ex)
            {
                logger.LogError(ex, ex.Message);
                if (!tcs.Task.IsCompleted)
                    tcs.SetResult(StatusCode(500, ex.Message));
            }
        }

        protected void HandleConsumerAction<T>(
            ILogger logger,
            ref BasicDeliverEventArgs evtArgs,
            ref TaskCompletionSource<IActionResult> tcs)
        {
            try
            {
                if (evtArgs.BasicProperties.CorrelationId == correlationID)
                {
                    _channel.BasicAck(deliveryTag: evtArgs.DeliveryTag, multiple: false);
                    string respStr = Encoding.UTF8.GetString(evtArgs.Body.ToArray());
                    var response = JsonSerializer.Deserialize<Response>(respStr);
                    tcs.SetResult(ProcessResponse<T>(response));
                }
                else
                {
                    _channel.BasicReject(deliveryTag: evtArgs.DeliveryTag, requeue: true);
                }
            }
            catch (RabbitMQOperationInterruptedException ex)
            {
                logger.LogError(ex, ex.Message);
                if (!tcs.Task.IsCompleted)
                    tcs.SetResult(StatusCode(500, ex.Message));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                if (!tcs.Task.IsCompleted)
                    tcs.SetResult(StatusCode(500, ex.Message));
            }
        }

        protected void HandleConsumerAction(
            ILogger logger,
            ref BasicDeliverEventArgs evtArgs,
            ref TaskCompletionSource<IActionResult> tcs)
        {
            try
            {
                if (evtArgs.BasicProperties.CorrelationId == correlationID)
                {
                    _channel.BasicAck(deliveryTag: evtArgs.DeliveryTag, multiple: false);
                    string respStr = Encoding.UTF8.GetString(evtArgs.Body.ToArray());
                    var response = JsonSerializer.Deserialize<Response>(respStr);
                    tcs.SetResult(ProcessResponse(response));
                }
                else
                {
                    _channel.BasicReject(deliveryTag: evtArgs.DeliveryTag, requeue: true);
                }
            }
            catch (RabbitMQOperationInterruptedException ex)
            {
                logger.LogError(ex, ex.Message);
                if (!tcs.Task.IsCompleted)
                    tcs.SetResult(StatusCode(500, ex.Message));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                if (!tcs.Task.IsCompleted)
                    tcs.SetResult(StatusCode(500, ex.Message));
            }
        }
    }
}
