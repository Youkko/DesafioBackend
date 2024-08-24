using Microsoft.AspNetCore.Mvc;
using MotorcycleRental.Models;
using MotorcycleRental.Models.DTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using MotorcycleRental.Models.Errors;
namespace MotorcycleRentalApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : CustomControllerBase
    {
        #region Object Instances
        private readonly ILogger<AuthController> _logger;
        #endregion
        
        #region Constructor
        public AuthController(
            ILogger<AuthController> logger,
            IConnection connection,
            IModel channel)
            : base(connection, channel)
        {
            _logger = logger;

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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin data)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, evtArgs) =>
                HandleConsumerAction<LoginResponse>(_logger, ref evtArgs, ref tcs);

            SendMessageAndListenForResponse(
                JsonSerializer.Serialize(data),
                Commands.AUTHENTICATE,
                Queues.DPS_AUTHIN,
                Queues.DPS_AUTHOUT,
                _logger,
                ref consumer,
                ref tcs);

            return await tcs.Task;
        }
    }
}