namespace Tweet.API.Infrastructure.Configurations.RabbitMqConfig
{
    /// <summary>
    /// Class for RabbitMq configuration
    /// </summary>
    public class RabbitMqConfiguration
    {
        /// <summary>
        /// Gets or sets the Hostname.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the QueueName.
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Gets or sets the UserName.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the Password.
        /// </summary>
        public string Password { get; set; }

    }
}
