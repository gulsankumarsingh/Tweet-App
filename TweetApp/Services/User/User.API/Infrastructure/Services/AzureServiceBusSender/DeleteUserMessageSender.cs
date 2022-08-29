using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.API.Infrastructure.Configurations.AzureBusConfig;
using User.API.Infrastructure.Services.AzureServiceBusSender.Interface;
using User.API.Models.Messages;

namespace User.API.Infrastructure.Services.AzureServiceBusSender
{
    public class DeleteUserMessageSender : IDeleteUserMessageSender
    {
        private readonly ILogger<DeleteUserMessageSender> _logger;
        private readonly string _queueConnectionString;
        private readonly string _topicName;

        public DeleteUserMessageSender(ILogger<DeleteUserMessageSender> logger, IOptions<AzureBusConfiguration> azureBusConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queueConnectionString = azureBusConfiguration.Value.QueueConnectionString;
            _topicName = azureBusConfiguration.Value.TopicName;
        }

        public async Task PublishMessage(BaseMessage baseMessage)
        {
            try
            {
                _logger.LogInformation($"Starting PublishMessage for queue id: {baseMessage.QueueId}");
                await using ServiceBusClient client = new ServiceBusClient(_queueConnectionString);
                
                ServiceBusSender sender = client.CreateSender(_topicName);

                string jsonMessage = JsonConvert.SerializeObject(baseMessage);
                ServiceBusMessage serializedContents = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
                {
                    CorrelationId = Guid.NewGuid().ToString()
                };
                await sender.SendMessageAsync(serializedContents);
                await client.DisposeAsync();
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while sending the message for queue id: {baseMessage.QueueId}", ex.Message);
            }
        }
    }
}
