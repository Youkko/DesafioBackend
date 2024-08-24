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
namespace OrderManagementService
{
    public class MessengerService : IHostedService
    {
        #region Object Instances
        private readonly ILogger<MessengerService> _logger;
        private readonly IOrderOps _orders;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQSettings _settings;
        private readonly Dictionary<string, Func<ReadOnlyMemory<byte>, string>>
            actionMap;
        #endregion

        #region Constructor
        public MessengerService(
            IOptions<RabbitMQSettings> options,
            ILogger<MessengerService> logger,
            IOrderOps orders,
            IDatabase database)
        {
            _logger = logger;
            _settings = options.Value;
            _orders = orders;
            _connection = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            }.CreateConnection();

            _channel = _connection.CreateModel();
            DeclareQueues();
            actionMap = new()
            {
                { Commands.LISTRENTALS, ListRentals },
                { Commands.LISTPLANS, ListPlans },
                { Commands.HIREVEHICLE, HireVehicle },
                { Commands.RETURNVEHICLE, SimulateVehicleReturn },
            };
        }
        #endregion

        #region Private methods

        #region -> Action methods
        private string ListRentals(ReadOnlyMemory<byte> body)
        {
            Response? response = null;
            try
            {
                response = _orders.ListUserRentals(Encoding.UTF8.GetString(body.ToArray()));
            }
            catch (Exception ex)
            {
                response = new(ex.Message, false);
                _logger.LogError(ex, ex.Message);
                return JS.JsonSerializer.Serialize(response);
            }
            return JS.JsonSerializer.Serialize(response);
        }

        private string ListPlans(ReadOnlyMemory<byte> body)
        {
            Response? response = null;
            try
            {
                response = _orders.ListPlans();
            }
            catch (Exception ex)
            {
                response = new(ex.Message, false);
                _logger.LogError(ex, ex.Message);
                return JS.JsonSerializer.Serialize(response);
            }
            return JS.JsonSerializer.Serialize(response);
        }

        private string HireVehicle(ReadOnlyMemory<byte> body)
        {
            try
            {
                var data = DeserializeMessage<RentalParams>(body.ToArray());
                var result = _orders.HireVehicle(data!).Result;
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

        private string SimulateVehicleReturn(ReadOnlyMemory<byte> body)
        {
            try
            {
                var data = DeserializeMessage<ReturnUserParams>(body.ToArray());
                var response = _orders.PreviewVehicleReturn(data!);
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
        #endregion

        #region -> Messaging system
        private void DeclareQueues()
        {
            foreach (
                var queue
                in Queues.All.Where(q =>
                    q.Key.StartsWith("oms_", StringComparison.InvariantCultureIgnoreCase)))
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

        private void OnMessageReceived(object? sender, BasicDeliverEventArgs e) =>
            RedirectToMap(actionMap, e);

        private void RedirectToMap(
            Dictionary<string, Func<ReadOnlyMemory<byte>, string>> map,
            BasicDeliverEventArgs e)
        {
            string request = GetRequest(e);
            if (!string.IsNullOrEmpty(request))
            {
                try
                {
                    if (map.TryGetValue(request, out var action))
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

            inConsumer.Received += OnMessageReceived;

            _channel.BasicConsume(queue: Queues.OMS_IN,
                                    autoAck: true,
                                    consumer: inConsumer);
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