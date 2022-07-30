namespace Tweet.API.Models.Dtos
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="CommentDto" />.
    /// </summary>
    public class CommentDto
    {
        /// <summary>
        /// Gets or sets the Message.
        /// </summary>
        [Required]
        [MaxLength(144, ErrorMessage = "Only upto 144 characters allowed.")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the TweetId.
        /// </summary>
        [Required]
        public int TweetId { get; set; }

        /// <summary>
        /// Gets or sets the UserName.
        /// </summary>
        [Required]
        public string UserName { get; set; }
    }
}
