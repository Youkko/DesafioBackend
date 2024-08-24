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
    public class UsersController : CustomControllerBase
    {
        #region Object Instances
        private readonly ILogger<UsersController> _logger;
        #endregion

        #region Constructor
        public UsersController(
            ILogger<UsersController> logger,
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
        [HttpPost()]
        public async Task<IActionResult> Add([FromBody] CreateUserParams data)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, evtArgs) =>
                HandleConsumerAction<CreatedUser>(_logger, ref evtArgs, ref tcs);

            SendMessageAndListenForResponse(
                JsonSerializer.Serialize(data),
                Commands.CREATEUSER,
                Queues.DPS_IN,
                Queues.DPS_OUT,
                _logger,
                ref consumer,
                ref tcs);

            return await tcs.Task;
        }

        [HttpPost("uploadcnh")]
        [Authorize]
        public async Task<IActionResult> UploadCNH(IFormFile data)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value??null;
            if (!string.IsNullOrEmpty(userId))
            {
                if (data == null || (data.ContentType != "image/png" && data.ContentType != "image/bmp"))
                {
                    tcs.SetResult(BadRequest(new InvalidFileExtensionException().Message));
                }
                else
                {
                    var consumer = new EventingBasicConsumer(_channel);

                    consumer.Received += (ModuleHandle, evtArgs) =>
                        HandleConsumerAction(_logger, ref evtArgs, ref tcs);

                    string b64 = string.Empty;
                    using (var ms = new MemoryStream())
                    {
                        await data.CopyToAsync(ms);
                        var bytes = ms.ToArray();
                        b64 = Convert.ToBase64String(bytes);
                    }
                    var uploadContents = new UploadFileParams(Guid.Parse(userId), b64, data.ContentType == "image/png" ? "png" : "bmp");

                    SendMessageAndListenForResponse(
                        JsonSerializer.Serialize(uploadContents),
                        Commands.UPLOADCNHIMAGE,
                        Queues.DPS_MANAGEIN,
                        Queues.DPS_MANAGEOUT,
                        _logger,
                        ref consumer,
                        ref tcs);
                }
            }
            else
                tcs.SetResult(Unauthorized());
            return await tcs.Task;
        }
        #endregion
    }
}