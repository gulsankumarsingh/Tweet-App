namespace Tweet.API.Infrastructure.Filters
{
    /// <summary>
    /// Defines the <see cref="JsonErrorResponse" />.
    /// </summary>
    public class JsonErrorResponse
    {
        /// <summary>
        /// Gets or sets the Messages.
        /// </summary>
        public string[] Messages { get; set; }

        /// <summary>
        /// Gets or sets the DeveloperMessage.
        /// </summary>
        public object DeveloperMessage { get; set; }
    }
}
