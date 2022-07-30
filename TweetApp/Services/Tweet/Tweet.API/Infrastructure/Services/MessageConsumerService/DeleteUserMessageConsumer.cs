namespace Tweet.API.Infrastructure.Services.MessageConsumerService
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Tweet.API.Infrastructure.Configurations.RabbitMqConfig;
    using Tweet.API.Infrastructure.Repository.Interface;
    using Tweet.API.Models;

    /// <summary>
    /// Defines the <see cref="DeleteUserMessageConsumer" />.
    /// </summary>
    public class DeleteUserMessageConsumer : BackgroundService
    {
        /// <summary>
        /// Defines the _channel.
        /// </summary>
        private IModel _channel;

        /// <summary>
        /// Defines the _connection.
        /// </summary>
        private IConnection _connection;

        /// <summary>
        /// Defines the _hostname.
        /// </summary>
        private readonly string _hostname;

        /// <summary>
        /// Defines the _queueName.
        /// </summary>
        private readonly string _queueName;

        /// <summary>
        /// Defines the _username.
        /// </summary>
        private readonly string _username;

        /// <summary>
        /// Defines the _password.
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        private readonly ILogger<DeleteUserMessageConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteUserMessageConsumer"/> class.
        /// </summary>
        /// <param name="rabbitMqOptions">The rabbitMq configuration<see cref="RabbitMqConfiguration"/>.</param>
        /// <param name="serviceProvider">The serviceProvider<see cref="IServiceProvider"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{DeleteUserMessageConsumer}"/>.</param>
        public DeleteUserMessageConsumer(IOptions<RabbitMqConfiguration> rabbitMqOptions,  ILogger<DeleteUserMessageConsumer> logger, IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _hostname = rabbitMqOptions.Value.Hostname;
            _queueName = rabbitMqOptions.Value.QueueName;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
            InitializeRabbitMqListener();
        }

        /// <summary>
        /// The InitializeRabbitMqListener.
        /// </summary>
        private void InitializeRabbitMqListener()
        {
            try
            {
                _logger.LogInformation("Started InitializeRabbitMqListener Method...");
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password,
                    DispatchConsumersAsync = true
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            }
            catch(Exception ex)
            {
                _logger.LogError("An error occured InitializeRabbitMqListener method", ex.Message);
            }
            
        }

        /// <summary>
        /// The ExecuteAsync.
        /// </summary>
        /// <param name="stoppingToken">The stoppingToken<see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Started ExecuteAsync Method...");
                stoppingToken.ThrowIfCancellationRequested();

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += Consumer_Received;

                _channel.BasicConsume(_queueName, false, consumer);
                _logger.LogInformation($"{_queueName} queue successfully consumed");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured in Consumer_Received method", ex.Message);
            }
            return Task.CompletedTask;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs ea)
        {
            try
            {
                _logger.LogInformation("Started Consumer_Received Method...");
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var deleteUserResultMessage = JsonConvert.DeserializeObject<DeleteUserResultMessage>(content);

                await HandleMessage(deleteUserResultMessage);

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {

                _logger.LogError("An error occured in Consumer_Received method", ex.Message);

            }

        }

        /// <summary>
        /// The HandleMessage.
        /// </summary>
        /// <param name="deleteUserResultMessage">The deleteUserResultMessage<see cref="DeleteUserResultMessage"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task HandleMessage(DeleteUserResultMessage deleteUserResultMessage)
        {
            try
            {
                _logger.LogInformation("Started HandleMessage Method...");
                using IServiceScope scope = _serviceProvider.CreateScope();
                var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ITweetRepository>();
                await scopedProcessingService.DeleteTweetByUserNameAsync(deleteUserResultMessage.UserName);
            }
            catch(Exception ex)
            {
                _logger.LogError("An error occured in HandleMessage method", ex.Message);
            }
            

        }

        /// <summary>
        /// The Dispose.
        /// </summary>
        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
