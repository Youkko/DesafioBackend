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
        }
        #endregion

        #region Private methods
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

        private void OnMgmtMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            var props = e.BasicProperties;

            if (props.Headers != null &&
                props.Headers.TryGetValue("request", out var operationBytes))
            {
                var operation = Encoding.UTF8.GetString((byte[])operationBytes);

                try
                {
                    switch (operation)
                    {
                        case "createvehicle":
                            PublishMessage(CreateMotorcycle(e.Body), e);
                            break;

                        case "editvin":
                            PublishMessage(EditVIN(e.Body), e);
                            break;
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
            var props = e.BasicProperties;

            if (props.Headers != null && 
                props.Headers.TryGetValue("request", out var operationBytes))
            {
                var operation = Encoding.UTF8.GetString((byte[])operationBytes);

                try
                {
                    switch (operation)
                    {
                        case "list":
                            PublishMessage(ListMotorcycles(e.Body), e);
                            break;
                        
                        case "notify":
                            Notify(e.Body);
                            break;
                    }
                }
                catch (RabbitMQOperationInterruptedException ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }
        }

        private string ListMotorcycles(ReadOnlyMemory<byte> body)
        {
            string message = Encoding.UTF8.GetString(body.ToArray());
            var motos = _vehicles.ListMotorcycles(message).Result;
            string requestMessage = JS.JsonSerializer.Serialize(motos);
            return requestMessage;
        }

        private string CreateMotorcycle(ReadOnlyMemory<byte> body)
        {
            string message = Encoding.UTF8.GetString(body.ToArray());
            var creationRequest = JsonConvert.DeserializeObject<MotorcycleCreation>(message);
            if (creationRequest == null)
                throw new RequiredInformationMissingException();
            var result = _vehicles.CreateVehicle(creationRequest).Result;

            if (result != null)
            {
                if (result.Year == 2024)
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
                        _logger.LogError(ex, ex.Message);
                        throw;
                    }
                }
            }

            return JS.JsonSerializer.Serialize(result);
        }

        private string EditVIN(ReadOnlyMemory<byte> body)
        {
            string message = Encoding.UTF8.GetString(body.ToArray());
            var editRequest = JsonConvert.DeserializeObject<VINEditionParams>(message);
            if (editRequest == null)
                throw new RequiredInformationMissingException();
            var result = _vehicles.EditVIN(editRequest).Result;
            return JS.JsonSerializer.Serialize(result);
        }

        private void Notify(ReadOnlyMemory<byte> body)
        {
            string message = Encoding.UTF8.GetString(body.ToArray());
            _vehicles.Notify(message);
        }
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