using Microsoft.AspNetCore.Mvc;
using MotorcycleRental.Models;
using MotorcycleRental.Models.DTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
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
            {
                try
                {
                    if (evtArgs.BasicProperties.CorrelationId == correlationID)
                    {
                        _channel.BasicAck(deliveryTag: evtArgs.DeliveryTag, multiple: false);
                        string respStr = Encoding.UTF8.GetString(evtArgs.Body.ToArray());
                        var response = JsonSerializer.Deserialize<Response>(respStr);
                        tcs.SetResult(ProcessResponse<LoginResponse>(response));
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
                    Commands.AUTHENTICATE,
                    Queues.DPS_AUTHIN,
                    Queues.DPS_AUTHOUT,
                    ref consumer);
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