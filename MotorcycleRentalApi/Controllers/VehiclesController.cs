using Microsoft.AspNetCore.Mvc;
using MotorcycleRental.Models;
using MotorcycleRental.Models.DTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
using MotorcycleRental.Models.Errors;
using Microsoft.AspNetCore.Authorization;
namespace MotorcycleRentalApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehiclesController : ControllerBase
    {
        #region Object Instances
        private readonly ILogger<VehiclesController> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private string? correlationID;
        #endregion

        #region Constructor
        public VehiclesController(
            ILogger<VehiclesController> logger)
        {
            _logger = logger;

            _connection = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER"),
                Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS")
            }.CreateConnection();

            _channel = _connection.CreateModel();

            foreach (var queue in Queues.All)
            {
                _channel.QueueDeclare(
                    queue: queue.Value,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            }
        }
        #endregion

        #region Private methods
        private void SendMessageAndListenForResponse(
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
        #endregion

        #region Endpoints
        [HttpGet()]
        [Authorize]
        public async Task<IActionResult> List([FromQuery] SearchVehicleParams data)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, evtArgs) =>
            {
                try
                {
                    if (evtArgs.BasicProperties.CorrelationId == correlationID)
                    {
                        _channel.BasicAck(deliveryTag: evtArgs.DeliveryTag, multiple: false);
                        string respStr = Encoding.UTF8.GetString(evtArgs.Body.ToArray());
                        var response = JsonSerializer.Deserialize<Response>(respStr);
                        if (response != null)
                        {
                            if (response.Success)
                            {
                                var results = JsonSerializer.Deserialize<IEnumerable<Motorcycle>?>(response!.Message!);
                                tcs.SetResult(Ok(results));
                            }
                            else
                            {
                                tcs.SetResult(StatusCode(500, response));
                            }
                        }
                        else
                        {
                            tcs.SetResult(StatusCode(500, response));
                        }
                    }
                    else
                    {
                        _channel.BasicReject(deliveryTag: evtArgs.DeliveryTag, requeue: true);
                    }
                }
                catch (RabbitMQOperationInterruptedException ex)
                {
                    _logger.LogError(ex, ex.Message);
                    tcs.SetResult(StatusCode(500, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    tcs.SetResult(StatusCode(500, ex.Message));
                }
            };

            try
            {
                SendMessageAndListenForResponse(
                    JsonSerializer.Serialize(data),
                    Commands.LISTVEHICLES,
                    Queues.MRS_IN,
                    Queues.MRS_OUT,
                    ref consumer);
            }
            catch (RabbitMQOperationInterruptedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

            return await tcs.Task;
        }

        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] CreateVehicleParams data)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, evtArgs) =>
            {
                try
                {
                    if (evtArgs.BasicProperties.CorrelationId == correlationID)
                    {
                        _channel.BasicAck(deliveryTag: evtArgs.DeliveryTag, multiple: false);
                        string respStr = Encoding.UTF8.GetString(evtArgs.Body.ToArray());
                        var response = JsonSerializer.Deserialize<Response>(respStr);
                        if (response != null)
                        {
                            if (response.Success)
                            {
                                var results = JsonSerializer.Deserialize<Motorcycle>(response!.Message!);
                                tcs.SetResult(Ok(results));
                            }
                            else
                            {
                                tcs.SetResult(StatusCode(500, response));
                            }
                        }
                        else
                        {
                            tcs.SetResult(StatusCode(500, response));
                        }
                    }
                    else
                    {
                        _channel.BasicReject(deliveryTag: evtArgs.DeliveryTag, requeue: true);
                    }
                }
                catch (RabbitMQOperationInterruptedException ex)
                {
                    _logger.LogError(ex, ex.Message);
                    tcs.SetResult(StatusCode(500, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    tcs.SetResult(StatusCode(500, ex.Message));
                }
            };

            try
            {
                SendMessageAndListenForResponse(
                    JsonSerializer.Serialize(data),
                    Commands.CREATEVEHICLE,
                    Queues.MRS_MANAGEIN,
                    Queues.MRS_MANAGEOUT,
                    ref consumer);
            }
            catch (RabbitMQOperationInterruptedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
            return await tcs.Task;
        }


        [HttpPatch()]
        [Authorize]
        public async Task<IActionResult> Edit([FromBody] EditVehicleParams data)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, evtArgs) =>
            {
                try
                {
                    if (evtArgs.BasicProperties.CorrelationId == correlationID)
                    {
                        _channel.BasicAck(deliveryTag: evtArgs.DeliveryTag, multiple: false);
                        string respStr = Encoding.UTF8.GetString(evtArgs.Body.ToArray());
                        var response = JsonSerializer.Deserialize<Response>(respStr);
                        if (response != null)
                        {
                            if (response.Success)
                            {
                                var results = JsonSerializer.Deserialize<Motorcycle>(response!.Message!);
                                tcs.SetResult(Ok(results));
                            }
                            else
                            {
                                tcs.SetResult(StatusCode(500, response));
                            }
                        }
                        else
                        {
                            tcs.SetResult(StatusCode(500, response));
                        }
                    }
                    else
                    {
                        _channel.BasicReject(deliveryTag: evtArgs.DeliveryTag, requeue: true);
                    }
                }
                catch (RabbitMQOperationInterruptedException ex)
                {
                    _logger.LogError(ex, ex.Message);
                    tcs.SetResult(StatusCode(500, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    tcs.SetResult(StatusCode(500, ex.Message));
                }
            };

            try
            {
                SendMessageAndListenForResponse(
                    JsonSerializer.Serialize(data),
                    Commands.EDITVIN,
                    Queues.MRS_MANAGEIN,
                    Queues.MRS_MANAGEOUT,
                    ref consumer);
            }
            catch (RabbitMQOperationInterruptedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

            return await tcs.Task;
        }

        [HttpDelete()]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] DeleteVehicleParams data)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, evtArgs) =>
            {
                try
                {
                    if (evtArgs.BasicProperties.CorrelationId == correlationID)
                    {
                        _channel.BasicAck(deliveryTag: evtArgs.DeliveryTag, multiple: false);
                        string respStr = Encoding.UTF8.GetString(evtArgs.Body.ToArray());
                        var response = JsonSerializer.Deserialize<Response>(respStr);
                        if (response != null)
                        {
                            if (response.Success)
                            {
                                tcs.SetResult(Ok(response.Success));
                            }
                            else
                            {
                                tcs.SetResult(StatusCode(500, response));
                            }
                        }
                        else
                        {
                            tcs.SetResult(StatusCode(500, response));
                        }

                    }
                    else
                    {
                        _channel.BasicReject(deliveryTag: evtArgs.DeliveryTag, requeue: true);
                    }
                }
                catch (RabbitMQOperationInterruptedException ex)
                {
                    _logger.LogError(ex, ex.Message);
                    tcs.SetResult(StatusCode(500, ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    tcs.SetResult(StatusCode(500, ex.Message));
                }
            };

            try
            {
                SendMessageAndListenForResponse(
                    JsonSerializer.Serialize(data),
                    Commands.DELETEVEHICLE,
                    Queues.MRS_MANAGEIN,
                    Queues.MRS_MANAGEOUT,
                    ref consumer);
            }
            catch (RabbitMQOperationInterruptedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

            return await tcs.Task;
        }
        #endregion
    }
}