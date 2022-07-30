namespace Tweet.API.Models.Messages
{
    using System;

    /// <summary>
    /// Defines the <see cref="BaseMessage" />.
    /// </summary>
    public class BaseMessage
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the MessageCreated.
        /// </summary>
        public DateTime MessageCreated { get; set; }
    }
}
