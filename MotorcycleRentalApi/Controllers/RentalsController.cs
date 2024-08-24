using Microsoft.AspNetCore.Mvc;
using MotorcycleRental.Models;
using MotorcycleRental.Models.DTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using MotorcycleRental.Models.Errors;
using Microsoft.AspNetCore.Authorization;
namespace MotorcycleRentalApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RentalsController : CustomControllerBase
    {
        #region Object Instances
        private readonly ILogger<RentalsController> _logger;
        #endregion

        #region Constructor
        public RentalsController(
            ILogger<RentalsController> logger,
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

        #region Endpoints
        [HttpGet()]
        [Authorize]
        public async Task<IActionResult> ListRentals()
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? null;
            if (!string.IsNullOrEmpty(userId))
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (ModuleHandle, evtArgs) =>
                    HandleConsumerAction<ICollection<RentalInfo>>(_logger, ref evtArgs, ref tcs);

                SendMessageAndListenForResponse(
                    userId,
                    Commands.LISTRENTALS,
                    Queues.OMS_IN,
                    Queues.OMS_OUT,
                    _logger,
                    ref consumer,
                    ref tcs);
            }
            else
                tcs.SetResult(Unauthorized());

            return await tcs.Task;
        }


        [HttpGet("ListPlans")]
        [Authorize]
        public async Task<IActionResult> ListPlans()
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, evtArgs) =>
                HandleConsumerAction<IEnumerable<RentalPlan>>(_logger, ref evtArgs, ref tcs);

            SendMessageAndListenForResponse(
                string.Empty,
                Commands.LISTPLANS,
                Queues.OMS_IN,
                Queues.OMS_OUT,
                _logger,
                ref consumer,
                ref tcs);

            return await tcs.Task;
        }

        [HttpPost("Hire")]
        [Authorize]
        public async Task<IActionResult> Hire([FromBody] HireParams data)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? null;
            if (!string.IsNullOrEmpty(userId))
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (ModuleHandle, evtArgs) =>
                    HandleConsumerAction<Rental>(_logger, ref evtArgs, ref tcs);

                var rentalParams = new RentalParams(Guid.Parse(userId), data.VIN!, data.PlanDays);
                SendMessageAndListenForResponse(
                    JsonSerializer.Serialize(rentalParams),
                    Commands.HIREVEHICLE,
                    Queues.OMS_IN,
                    Queues.OMS_OUT,
                    _logger,
                    ref consumer,
                    ref tcs);
            }
            else
                tcs.SetResult(Unauthorized());
            return await tcs.Task;
        }

        [HttpPost("Return")]
        [Authorize]
        public async Task<IActionResult> Return([FromBody] ReturnParams data)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? null;
            if (!string.IsNullOrEmpty(userId))
            {
                var returnUserParams = new ReturnUserParams()
                {
                    ReturnDate = data.ReturnDate,
                    VIN = data.VIN,
                    UserId = Guid.Parse(userId)
                };
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (ModuleHandle, evtArgs) =>
                    HandleConsumerAction<ReturnInfo>(_logger, ref evtArgs, ref tcs);

                SendMessageAndListenForResponse(
                    JsonSerializer.Serialize(returnUserParams),
                    Commands.RETURNVEHICLE,
                    Queues.OMS_IN,
                    Queues.OMS_OUT,
                    _logger,
                    ref consumer,
                    ref tcs);
            }
            else
                tcs.SetResult(Unauthorized());
            return await tcs.Task;
        }
        #endregion
    }
}