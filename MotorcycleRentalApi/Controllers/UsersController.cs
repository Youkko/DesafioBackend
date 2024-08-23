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
        /*
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
                        tcs.SetResult(ProcessResponse<IEnumerable<Motorcycle>?>(response));
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
        */

        [HttpPost()]
        public async Task<IActionResult> Add([FromBody] CreateUserParams data)
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
                        tcs.SetResult(ProcessResponse<CreatedUser>(response));
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
                    Commands.CREATEUSER,
                    Queues.DPS_IN,
                    Queues.DPS_OUT,
                    ref consumer);
            }
            catch (RabbitMQOperationInterruptedException ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
            return await tcs.Task;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFile data)
        {
            var tcs = new TaskCompletionSource<IActionResult>();
            if (data == null || (data.ContentType != "image/png" && data.ContentType != "image/bmp"))
            {
                tcs.SetResult(BadRequest(new InvalidFileExtensionException().Message));
            }
            else
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value??null;
                if (!string.IsNullOrEmpty(userId))
                {
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
                                tcs.SetResult(ProcessResponse(response));
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

                    string b64 = string.Empty;
                    using (var ms = new MemoryStream())
                    {
                        await data.CopyToAsync(ms);
                        var bytes = ms.ToArray();
                        b64 = Convert.ToBase64String(bytes);
                    }
                    var uploadContents = new UploadFileParams(Guid.Parse(userId), b64, data.ContentType == "image/png" ? "png" : "bmp");

                    try
                    {
                        SendMessageAndListenForResponse(
                            JsonSerializer.Serialize(uploadContents),
                            Commands.UPLOADCNHIMAGE,
                            Queues.DPS_MANAGEIN,
                            Queues.DPS_MANAGEOUT,
                            ref consumer);
                    }
                    catch (RabbitMQOperationInterruptedException ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        return StatusCode(500, ex.Message);
                    }
                }
                else
                    tcs.SetResult(Unauthorized());
            }
            return await tcs.Task;
        }

        /*
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
                        tcs.SetResult(ProcessResponse<Motorcycle>(response));
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
                        tcs.SetResult(ProcessResponse(response));
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
        */
        #endregion
    }
}