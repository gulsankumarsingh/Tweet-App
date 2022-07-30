namespace User.API.Models
{
    /// <summary>
    /// Defines the <see cref="ApiResponse" />.
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the ResponseValue.
        /// </summary>
        public object ResponseValue { get; set; } = null;
    }
}
