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
namespace DeliveryPersonService
{
    public class MessengerService : IHostedService
    {
        #region Object Instances
        private readonly ILogger<MessengerService> _logger;
        private readonly IAuthentication _auth;
        private readonly IUserOps _users;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQSettings _settings;
        private readonly Dictionary<string, Func<ReadOnlyMemory<byte>, string>>
            actionMap,
            mgmtActionMap,
            authActionMap;
        private readonly string cnhFilePath;
        #endregion

        #region Constructor
        public MessengerService(
            IOptions<RabbitMQSettings> options,
            ILogger<MessengerService> logger,
            IAuthentication auth,
            IUserOps users,
            IDatabase database)
        {
            cnhFilePath = "/app/data/CNH";
            _logger = logger;
            _auth = auth;
            _users = users;
            _settings = options.Value;
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
                { Commands.UPLOADCNHIMAGE, CreateFile },
            };
            actionMap = new()
            {
                { Commands.CREATEUSER, CreateUser },
            };
            authActionMap = new()
            {
                { Commands.AUTHENTICATE, AuthenticateClient },
            };
        }
        #endregion

        #region Private methods

        #region -> Action methods
        private string AuthenticateClient(ReadOnlyMemory<byte> body)
        {
            Response? response = null;
            try
            {
                var data = DeserializeMessage<UserLogin>(body.ToArray());
                response = _auth.AuthenticateAsync(data!).Result;
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
            return JS.JsonSerializer.Serialize(response);
        }

        private string CreateUser(ReadOnlyMemory<byte> body)
        {
            Response? response = null;
            try
            {
                var data = DeserializeMessage<CreateUserParams>(body.ToArray());
                response = _users.CreateUser(data!).Result;
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
            return JS.JsonSerializer.Serialize(response);
        }

        private string CreateFile(ReadOnlyMemory<byte> body)
        {
            Response? response = null;
            try
            {
                var data = DeserializeMessage<UploadFileParams>(body.ToArray());
                var fileBytes = Convert.FromBase64String(data!.FileContents!);
                var filePath = Path.Combine(cnhFilePath, $"{data.UserID.ToString()}.{data.Format}");
                if (!Directory.Exists(cnhFilePath))
                {
                    Directory.CreateDirectory(cnhFilePath);
                }
                File.WriteAllBytes(filePath, fileBytes);
                response = new(null, true);
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
            return JS.JsonSerializer.Serialize(response);
        }

        #endregion

        #region -> Messaging system
        private void DeclareQueues()
        {
            foreach (
                var queue
                in Queues.All.Where(q =>
                    q.Key.StartsWith("dps_", StringComparison.InvariantCultureIgnoreCase)))
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

        private void OnMgmtMessageReceived(object? sender, BasicDeliverEventArgs e) =>
            RedirectToMap(mgmtActionMap, e);

        private void OnMessageReceived(object? sender, BasicDeliverEventArgs e) =>
            RedirectToMap(actionMap, e);

        private void OnAuthMessageReceived(object? sender, BasicDeliverEventArgs e) =>
            RedirectToMap(authActionMap, e);

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
            var authConsumer = new EventingBasicConsumer(_channel);
            var inConsumer = new EventingBasicConsumer(_channel);
            var inMgmtConsumer = new EventingBasicConsumer(_channel);

            authConsumer.Received += OnAuthMessageReceived;
            inConsumer.Received += OnMessageReceived;
            inMgmtConsumer.Received += OnMgmtMessageReceived;

            _channel.BasicConsume(queue: Queues.DPS_AUTHIN,
                                    autoAck: true,
                                    consumer: authConsumer);

            _channel.BasicConsume(queue: Queues.DPS_IN,
                                    autoAck: true,
                                    consumer: inConsumer);

            _channel.BasicConsume(queue: Queues.DPS_MANAGEIN,
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