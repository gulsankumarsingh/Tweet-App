namespace Tweet.API.Models.Dtos
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="MessageDto" />.
    /// </summary>
    public class MessageDto
    {
        /// <summary>
        /// Gets or sets the Message.
        /// </summary>
        [Required]
        [MaxLength(144, ErrorMessage = "Only upto 144 characters allowed.")]
        public string Message { get; set; }
    }
}
