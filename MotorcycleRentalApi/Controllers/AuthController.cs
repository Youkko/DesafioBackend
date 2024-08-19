using Microsoft.AspNetCore.Mvc;
using MotorcycleRental.Models;
using MotorcycleRental.Models.DTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Exceptions;
using MotorcycleRental.Models.Errors;
namespace MotorcycleRentalApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private string? correlationID;

        public AuthController(
            ILogger<AuthController> logger)
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            correlationID = Guid.NewGuid().ToString();
            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationID;
            props.ReplyTo = Queues.DPS_AUTHOUT;
            
            try
            {
                string serializedMsg = JsonSerializer.Serialize(userLogin);
                byte[] bytes = Encoding.UTF8.GetBytes(serializedMsg);
                _channel.BasicPublish(
                    string.Empty,
                    Queues.DPS_AUTHIN,
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
                        var response = JsonSerializer.Deserialize<LoginResponse>(
                            Encoding.UTF8.GetString(evtArgs.Body.ToArray()));

                        tcs.SetResult(response != null &&
                                      response.IsAuthenticated.HasValue &&
                                      response.IsAuthenticated.Value ?  
                                      Ok(response) :
                                      Unauthorized());

                        _channel.BasicAck(deliveryTag: evtArgs.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        _channel.BasicReject(deliveryTag: evtArgs.DeliveryTag, requeue: false);
                    }
                }
                catch (RabbitMQOperationInterruptedException ex)
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
        
        /*
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegister)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = userRegister.Name,
                Email = userRegister.Email,
                PhoneNumber = userRegister.PhoneNumber,
                Password = HashPassword(userRegister.Password),
                CreatedOn = DateTime.UtcNow
            };

            await _database.AddAsync(user);
            return Ok();
        }
        */
    }
}