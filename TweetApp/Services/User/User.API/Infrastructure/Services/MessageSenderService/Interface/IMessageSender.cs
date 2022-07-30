namespace User.API.Infrastructure.Services.MessageSenderService.Interface
{
    using User.API.Models.Messages;

    /// <summary>
    /// Defines the <see cref="IMessageSender" />.
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        /// Send Message.
        /// </summary>
        /// <param name="baseMessage">The baseMessage<see cref="BaseMessage"/>.</param>
        void SendMessage(BaseMessage baseMessage);
    }
}
