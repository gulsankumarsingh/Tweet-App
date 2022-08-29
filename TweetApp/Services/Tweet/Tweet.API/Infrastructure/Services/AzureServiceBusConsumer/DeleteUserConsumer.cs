using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweet.API.Infrastructure.Configurations.AzureBusConfig;
using Tweet.API.Infrastructure.Repository.Interface;
using Tweet.API.Models;

namespace Tweet.API.Infrastructure.Services.AzureServiceBusConsumer
{
    public class DeleteUserConsumer : BackgroundService
    {
        private readonly ILogger<DeleteUserConsumer> _logger;
        private readonly ServiceBusProcessor _deleteUserProcessor;
        private readonly IServiceProvider _serviceProvider;

        public DeleteUserConsumer(IOptions<AzureBusConfiguration> azureBusConfiguration, ILogger<DeleteUserConsumer> logger, IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            var serviceBusConnectionString = azureBusConfiguration.Value.QueueConnectionString;
            var subscriptionDeleteUser = azureBusConfiguration.Value.UserDeletedSubscription;
            var deleteUserMessageTopic = azureBusConfiguration.Value.TopicName;

            var serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
            _deleteUserProcessor = serviceBusClient.CreateProcessor(deleteUserMessageTopic, subscriptionDeleteUser); //for topic
        }

        public async Task StartConsumer()
        {
            try
            {
                _logger.LogInformation("Starting StartConsumer method...");
                _deleteUserProcessor.ProcessMessageAsync += onDeleteUserMessageReceived;
                _deleteUserProcessor.ProcessErrorAsync += ErrorHandler;
                await _deleteUserProcessor.StartProcessingAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while stopping the consumer ", ex.Message);
            }
        }

        public async Task StopConsumer()
        {
            try
            {
                _logger.LogInformation("Starting the StopConsumber method...");
                await _deleteUserProcessor.StopProcessingAsync();
                await _deleteUserProcessor.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while stopping the consumer ", ex.Message);
            }
        }

        private async Task onDeleteUserMessageReceived(ProcessMessageEventArgs args)
        {
            try
            {
                _logger.LogInformation("Starting onDeleteUserMessageReceived method...");
                var message = args.Message;
                var body = Encoding.UTF8.GetString(message.Body);

                DeleteUserResultMessage deleteUserResultMessage = JsonConvert.DeserializeObject<DeleteUserResultMessage>(body);
                await HandleMessage(deleteUserResultMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured in onDeleteUserMessageReceived method ", ex.Message);
            }
         
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception.ToString());
            return Task.CompletedTask;
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
                await scopedProcessingService.DeleteLikeByUsernameAsync(deleteUserResultMessage.UserName);
                await scopedProcessingService.DeleteReplyByUsernameAsync(deleteUserResultMessage.UserName);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured in HandleMessage method", ex.Message);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartConsumer();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopConsumer();
        }

        /// <summary>
        /// The Dispose.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
    }

}
