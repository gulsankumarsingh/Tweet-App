using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet.API.Infrastructure.Configurations.AzureBusConfig
{
    public class AzureBusConfiguration
    {
        /// <summary>
        /// Gets or sets the QueueConnectionString.
        /// </summary>
        public string QueueConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the QueueName.
        /// </summary>
        public string TopicName { get; set; }

        public string UserDeletedSubscription { get; set; }
    }
}
