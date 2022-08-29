namespace User.API.Models.Messages
{
    using System;

    /// <summary>
    /// Defines the <see cref="BaseMessage" />.
    /// </summary>
    public class BaseMessage
    {
        /// <summary>
        /// Gets or sets the QueueId.
        /// </summary>
        public string QueueId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the MessageCreated.
        /// </summary>
        public DateTime MessageCreated { get; set; } = DateTime.Now;
    }
}
