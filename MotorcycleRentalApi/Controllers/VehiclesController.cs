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
        private readonly ILogger<VehiclesController> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private string? correlationID;

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

        [HttpGet()]
        [Authorize]
        public async Task<IActionResult> List([FromQuery] string? VIN)
        {
            try
            {
                correlationID = Guid.NewGuid().ToString();
                var props = _channel.CreateBasicProperties();
                props.CorrelationId = correlationID;
                props.ReplyTo = Queues.MRS_OUT;
                props.Headers = new Dictionary<string, object>();
                props.Headers.Add("request", "list");

                string serializedMsg = JsonSerializer.Serialize(VIN);
                byte[] bytes = Encoding.UTF8.GetBytes(serializedMsg);
                _channel.BasicPublish(
                    string.Empty,
                    Queues.MRS_IN,
                    props,
                    bytes);
            }
            catch (RabbitMQOperationInterruptedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

            var tcs = new TaskCompletionSource<IActionResult>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, evtArgs) =>
            {

                try
                {
                    if (evtArgs.BasicProperties.CorrelationId == correlationID)
                    {
                        string respStr = Encoding.UTF8.GetString(evtArgs.Body.ToArray());
                        var response = JsonSerializer.Deserialize<IEnumerable<Motorcycle>?>(respStr);
                        _channel.BasicAck(deliveryTag: evtArgs.DeliveryTag, multiple: false);
                        tcs.SetResult(Ok(response));
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
                _channel.BasicConsume(
                    queue: Queues.MRS_OUT,
                    autoAck: false,
                    consumer);
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
        public async Task<IActionResult> Add([FromBody] MotorcycleCreation vehicleData)
        {
            correlationID = Guid.NewGuid().ToString();
            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationID;
            props.ReplyTo = Queues.MRS_MANAGEOUT;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("request", "createvehicle");
            try
            {

                string serializedMsg = JsonSerializer.Serialize(vehicleData);
                byte[] bytes = Encoding.UTF8.GetBytes(serializedMsg);
                _channel.BasicPublish(
                    string.Empty,
                    Queues.MRS_MANAGEIN,
                    props,
                    bytes);
            }
            catch (RabbitMQOperationInterruptedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

            var tcs = new TaskCompletionSource<IActionResult>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, evtArgs) =>
            {
                try
                {
                    if (evtArgs.BasicProperties.CorrelationId == correlationID)
                    {
                        string respStr = Encoding.UTF8.GetString(evtArgs.Body.ToArray());
                        var response = JsonSerializer.Deserialize<Motorcycle?>(respStr);
                        _channel.BasicAck(deliveryTag: evtArgs.DeliveryTag, multiple: false);
                        tcs.SetResult(Ok(response));
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
                _channel.BasicConsume(
                    queue: props.ReplyTo,
                    autoAck: false,
                    consumer);
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
        public async Task<IActionResult> Edit([FromBody] VINEditionParams vinData)
        {
            correlationID = Guid.NewGuid().ToString();
            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationID;
            props.ReplyTo = Queues.MRS_MANAGEOUT;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("request", "editvin");
            try
            {

                string serializedMsg = JsonSerializer.Serialize(vinData);
                byte[] bytes = Encoding.UTF8.GetBytes(serializedMsg);
                _channel.BasicPublish(
                    string.Empty,
                    Queues.MRS_MANAGEIN,
                    props,
                    bytes);
            }
            catch (RabbitMQOperationInterruptedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

            var tcs = new TaskCompletionSource<IActionResult>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, evtArgs) =>
            {
                try
                {
                    if (evtArgs.BasicProperties.CorrelationId == correlationID)
                    {
                        string respStr = Encoding.UTF8.GetString(evtArgs.Body.ToArray());
                        var response = JsonSerializer.Deserialize<bool>(respStr);
                        _channel.BasicAck(deliveryTag: evtArgs.DeliveryTag, multiple: false);
                        tcs.SetResult(Ok(response));
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
                _channel.BasicConsume(
                    queue: props.ReplyTo,
                    autoAck: false,
                    consumer);
            }
            catch (RabbitMQOperationInterruptedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }

            return await tcs.Task;
        }
    }
}