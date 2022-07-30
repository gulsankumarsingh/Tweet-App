namespace User.API.Infrastructure.Services.MessageSenderService
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using System;
    using System.Text;
    using User.API.Infrastructure.Configurations.RabitMqConfig;
    using User.API.Infrastructure.Services.MessageSenderService.Interface;
    using User.API.Models.Messages;

    /// <summary>
    /// Defines the <see cref="MessageSender" />.
    /// </summary>
    public class MessageSender : IMessageSender
    {
        /// <summary>
        /// Defines the _hostname.
        /// </summary>
        private readonly string _hostname;

        /// <summary>
        /// Defines the _password.
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// Defines the _queueName.
        /// </summary>
        private readonly string _queueName;

        /// <summary>
        /// Defines the _username.
        /// </summary>
        private readonly string _username;

        /// <summary>
        /// Defines the _connection.
        /// </summary>
        private IConnection _connection;

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        private readonly ILogger<MessageSender> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSender"/> class.
        /// </summary>
        /// <param name="rabbitMqOptions">The rabbitMqOptions<see cref="IOptions{RabbitMqConfiguration}"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{MessageSender}"/>.</param>
        public MessageSender(IOptions<RabbitMqConfiguration> rabbitMqOptions, ILogger<MessageSender> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queueName = rabbitMqOptions.Value.QueueName;
            _hostname = rabbitMqOptions.Value.Hostname;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;

            CreateConnection();
        }

        /// <summary>
        /// The SendMessage.
        /// </summary>
        /// <param name="baseMessage">The baseMessage<see cref="BaseMessage"/>.</param>
        public void SendMessage(BaseMessage baseMessage)
        {
            try
            {
                _logger.LogInformation("Starting SendMessage method...");
                if (ConnectionExists())
                {
                    using (var channel = _connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                        var json = JsonConvert.SerializeObject(baseMessage);
                        var body = Encoding.UTF8.GetBytes(json);

                        channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError("An error occured while sending the message", ex.Message);
            }
            
        }

        /// <summary>
        /// The CreateConnection.
        /// </summary>
        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {

                _logger.LogError("An error occured while creating the connection", ex.Message);
            }
        }

        /// <summary>
        /// The ConnectionExists.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool ConnectionExists()
        {
            try
            {
                if (_connection != null)
                {
                    return true;
                }

                CreateConnection();
            }
            catch (Exception ex)
            {

                _logger.LogError("An error occured while checking the connection", ex.Message);
            }
            return _connection != null;
        }
    }
}
