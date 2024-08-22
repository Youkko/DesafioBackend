using DeliveryPersonService;
using Microsoft.Extensions.Options;
using MotorcycleRental.Data;
using MotorcycleRental.Models;
using MotorcycleRental.Models.DTO;
using MotorcycleRental.Models.Errors;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using JS = System.Text.Json;
namespace MotorcycleService
{
    public class MessengerService : IHostedService
    {
        #region Object Instances
        private readonly ILogger<MessengerService> _logger;
        private readonly IVehicleOps _vehicles;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQSettings _settings;
        private readonly Dictionary<string, Func<ReadOnlyMemory<byte>, string>>
            actionMap,
            mgmtActionMap;
        #endregion

        #region Constructor
        public MessengerService(
            IOptions<RabbitMQSettings> options,
            ILogger<MessengerService> logger,
            IVehicleOps vehicles,
            IDatabase database)
        {
            _logger = logger;
            _settings = options.Value;
            _vehicles = vehicles;
            _connection = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            }.CreateConnection();

            _channel = _connection.CreateModel();
            DeclareQueues();
            mgmtActionMap = new()
            {
                { Commands.CREATEVEHICLE, CreateMotorcycle },
                { Commands.EDITVIN, EditVIN },
                { Commands.DELETEVEHICLE, DeleteMotorcycle },
            };
            actionMap = new()
            {
                { Commands.LISTVEHICLES, ListMotorcycles },
                { Commands.NOTIFY, Notify },
            };
        }
        #endregion

        #region Private methods

        #region -> Action methods
        private string CreateMotorcycle(ReadOnlyMemory<byte> body)
        {
            Response? response = null;
            try
            {
                var data = DeserializeMessage<CreateVehicleParams>(body.ToArray());
                response = _vehicles.CreateVehicle(data!).Result;
            }
            catch (AggregateException aEx)
            {
                response = new(string.Empty, false);
                aEx.Flatten().Handle(ex =>
                {
                    response.Message = ex.Message;
                    _logger.LogError(ex, ex.Message);
                    return true;
                });
                return JS.JsonSerializer.Serialize(response);
            }

            if (response != null)
            {
                Motorcycle? result = JsonConvert.DeserializeObject<Motorcycle>(response.Message!.ToString()!);
                if (result != null && result.Year == 2024)
                {
                    try
                    {
                        var props = _channel.CreateBasicProperties();
                        props.CorrelationId = Guid.NewGuid().ToString();
                        props.ReplyTo = Queues.MRS_OUT;
                        props.Headers = new Dictionary<string, object>();
                        props.Headers.Add("request", "notify");

                        byte[] msg = Encoding.UTF8.GetBytes($"A new motorcycle with Year = 2024 was added! ID: {result.Id}");
                        _channel.BasicPublish(
                            string.Empty,
                            Queues.MRS_IN,
                            props,
                            msg
                        );
                    }
                    catch (RabbitMQOperationInterruptedException ex)
                    {
                        response = new(ex.Message, false);
                        _logger.LogError(ex, ex.Message);
                        return JS.JsonSerializer.Serialize(response);
                    }
                    catch (AggregateException aEx)
                    {
                        response = new(string.Empty, false);
                        aEx.Flatten().Handle(ex =>
                        {
                            response.Message = ex.Message;
                            _logger.LogError(ex, ex.Message);
                            return true;
                        });
                        return JS.JsonSerializer.Serialize(response);
                    }
                }
            }

            return JS.JsonSerializer.Serialize(response);
        }

        private string EditVIN(ReadOnlyMemory<byte> body)
        {
            try
            {
                var data = DeserializeMessage<EditVehicleParams>(body.ToArray());
                var result = _vehicles.EditVehicle(data!).Result;
                return JS.JsonSerializer.Serialize(result);
            }
            catch (AggregateException aEx)
            {
                Response response = new(string.Empty, false);
                aEx.Flatten().Handle(ex =>
                {
                    response.Message = ex.Message;
                    _logger.LogError(ex, ex.Message);
                    return true;
                });
                return JS.JsonSerializer.Serialize(response);
            }
        }

        private string DeleteMotorcycle(ReadOnlyMemory<byte> body)
        {
            try
            {
                var data = DeserializeMessage<DeleteVehicleParams>(body.ToArray());
                var response = _vehicles.DeleteVehicle(data!).Result;
                return JS.JsonSerializer.Serialize(response);
            }
            catch (AggregateException aEx)
            {
                Response response = new(string.Empty, false);
                aEx.Flatten().Handle(ex =>
                {
                    response.Message = ex.Message;
                    _logger.LogError(ex, ex.Message);
                    return true;
                });
                return JS.JsonSerializer.Serialize(response);
            }
        }

        private string ListMotorcycles(ReadOnlyMemory<byte> body)
        {
            try
            {
                var data = DeserializeMessage<SearchVehicleParams>(body.ToArray());
                var vehicles = _vehicles.ListVehicles(data!).Result;
                return JS.JsonSerializer.Serialize(vehicles);
            }
            catch (AggregateException aEx)
            {
                Response response = new(string.Empty, false);
                aEx.Flatten().Handle(ex =>
                {
                    response.Message = ex.Message;
                    _logger.LogError(ex, ex.Message);
                    return true;
                });
                return JS.JsonSerializer.Serialize(response);
            }
        }
        
        private string Notify(ReadOnlyMemory<byte> body)
        {
            try
            {
                string message = Encoding.UTF8.GetString(body.ToArray());
                _vehicles.Notify(message);
                return message;
            }
            catch (AggregateException aEx)
            {
                Response response = new(string.Empty, false);
                aEx.Flatten().Handle(ex =>
                {
                    response.Message = ex.Message;
                    _logger.LogError(ex, ex.Message);
                    return true;
                });
                return JS.JsonSerializer.Serialize(response);
            }
        }
        #endregion

        #region -> Messaging system
        private void DeclareQueues()
        {
            foreach (
                var queue
                in Queues.All.Where(q =>
                    q.Key.StartsWith("mrs_", StringComparison.InvariantCultureIgnoreCase)))
            {
                _channel.QueueDeclare(
                    queue: queue.Value,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            }
        }
        
        private void PublishMessage(string message, BasicDeliverEventArgs e)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                string.Empty,
                e.BasicProperties.ReplyTo,
                e.BasicProperties,
                msg
            );
        }

        private string GetRequest(BasicDeliverEventArgs args)
        {
            string request = string.Empty;
            var props = args.BasicProperties;
            if (props.Headers != null &&
                props.Headers.TryGetValue("request", out var operationBytes))
            {
                request = Encoding.UTF8.GetString((byte[])operationBytes);
            }
            return request;
        }

        private T? DeserializeMessage<T>(byte[] bMessage, bool required = true)
        {
            var response = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bMessage));
            if (required && response == null)
                throw new RequiredInformationMissingException();
            return response;
        }

        private void OnMgmtMessageReceived(object? sender, BasicDeliverEventArgs e)
        {

            string request = GetRequest(e);
            if (!string.IsNullOrEmpty(request))
            {
                try
                {
                    if (mgmtActionMap.TryGetValue(request, out var action))
                    {
                        string response = action(e.Body);
                        PublishMessage(response, e);
                    }
                    else
                    {
                        _logger.LogWarning("Unknown request: {Request}", request);
                    }
                }
                catch (RabbitMQOperationInterruptedException ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }
        }

        private void OnMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            string request = GetRequest(e);
            if (!string.IsNullOrEmpty(request))
            {
                try
                {
                    if (actionMap.TryGetValue(request, out var action))
                    {
                        string response = action(e.Body);
                        PublishMessage(response, e);
                    }
                    else
                    {
                        _logger.LogWarning("Unknown request: {Request}", request);
                    }
                }
                catch (RabbitMQOperationInterruptedException ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }
        }
        #endregion

        #endregion

        #region Public methods
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Consumer started at: {time}", DateTimeOffset.Now);
            }
            var inConsumer = new EventingBasicConsumer(_channel);
            var inMgmtConsumer = new EventingBasicConsumer(_channel);

            inConsumer.Received += OnMessageReceived;
            inMgmtConsumer.Received += OnMgmtMessageReceived;

            _channel.BasicConsume(queue: Queues.MRS_IN,
                                    autoAck: true,
                                    consumer: inConsumer);

            _channel.BasicConsume(queue: Queues.MRS_MANAGEIN,
                                    autoAck: true,
                                    consumer: inMgmtConsumer);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
            return Task.CompletedTask;
        }
        #endregion
    }
}