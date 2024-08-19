using Microsoft.Extensions.Options;
using MotorcycleRental.Data;
using MotorcycleRental.Models;
using MotorcycleRental.Models.Errors;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
namespace OrderManagementService
{
    public class MessengerService : IHostedService
    {
        #region Object Instances
        private readonly ILogger<MessengerService> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQSettings _settings;
        #endregion

        #region Constructor
        public MessengerService(
            IOptions<RabbitMQSettings> options,
            ILogger<MessengerService> logger,
            IDatabase database)
        {
            _logger = logger;
            _settings = options.Value;
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

        private void OnMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            string? message = Encoding.UTF8.GetString(body.ToArray());
            //var loginRequest = JsonConvert.DeserializeObject<UserLogin>(message);
            //var result = _auth.AuthenticateAsync(loginRequest!);
            try
            {
                //_channel.BasicPublish(
                //    string.Empty,
                //    Queues.AUTHOUT,
                //null,
                //    Encoding.UTF8.GetBytes(
                //        System.Text.Json.JsonSerializer.Serialize(result.Result)));
            }
            catch (RabbitMQOperationInterruptedException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        #endregion

        #region Public methods
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Consumer started at: {time}", DateTimeOffset.Now);
            }
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnMessageReceived;
            //_channel.BasicConsume(queue: Queues.AUTHIN,
            //                      autoAck: true,
            //                      consumer: consumer);
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