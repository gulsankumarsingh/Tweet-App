namespace User.API.Infrastructure.Filters
{
    /// <summary>
    /// Class for Json Error Response
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
