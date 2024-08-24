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
    public class VehiclesController : CustomControllerBase
    {
        #region Object Instances
        private readonly ILogger<VehiclesController> _logger;
        #endregion

        #region Constructor
        public VehiclesController(
            ILogger<VehiclesController> logger,
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
        public async Task<IActionResult> List([FromQuery] SearchVehicleParams data)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, evtArgs) =>
                HandleConsumerAction<IEnumerable<Vehicle>?>(_logger, ref evtArgs, ref tcs);

            SendMessageAndListenForResponse(
                JsonSerializer.Serialize(data),
                Commands.LISTVEHICLES,
                Queues.MRS_IN,
                Queues.MRS_OUT,
                _logger,
                ref consumer,
                ref tcs);

            return await tcs.Task;
        }

        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] CreateVehicleParams data)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, evtArgs) =>
                HandleConsumerAction<Vehicle>(_logger, ref evtArgs, ref tcs);

            SendMessageAndListenForResponse(
                JsonSerializer.Serialize(data),
                Commands.CREATEVEHICLE,
                Queues.MRS_MANAGEIN,
                Queues.MRS_MANAGEOUT,
                _logger,
                ref consumer,
                ref tcs);

            return await tcs.Task;
        }

        [HttpPatch()]
        [Authorize]
        public async Task<IActionResult> Edit([FromBody] EditVehicleParams data)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, evtArgs) =>
                HandleConsumerAction<Vehicle>(_logger, ref evtArgs, ref tcs);

            SendMessageAndListenForResponse(
                JsonSerializer.Serialize(data),
                Commands.EDITVIN,
                Queues.MRS_MANAGEIN,
                Queues.MRS_MANAGEOUT,
                _logger,
                ref consumer,
                ref tcs);

            return await tcs.Task;
        }

        [HttpDelete()]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] DeleteVehicleParams data)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, evtArgs) =>
                HandleConsumerAction(_logger, ref evtArgs, ref tcs);

            SendMessageAndListenForResponse(
                JsonSerializer.Serialize(data),
                Commands.DELETEVEHICLE,
                Queues.MRS_MANAGEIN,
                Queues.MRS_MANAGEOUT,
                _logger,
                ref consumer,
                ref tcs);

            return await tcs.Task;
        }
        #endregion
    }
}