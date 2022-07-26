﻿namespace User.API.Models
{
    using User.API.Models.Messages;

    /// <summary>
    /// Defines the <see cref="DeleteUserResultMessage" />.
    /// </summary>
    public class DeleteUserResultMessage : BaseMessage
    {
        /// <summary>
        /// Gets or sets the UserName.
        /// </summary>
        public string UserName { get; set; }
    }
}
