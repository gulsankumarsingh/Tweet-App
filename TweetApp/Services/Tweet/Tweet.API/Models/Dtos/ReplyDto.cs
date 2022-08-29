namespace Tweet.API.Models.Dtos
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="ReplyDto" />.
    /// </summary>
    public class ReplyDto
    {
        public string Id { get; set; }
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
        public string TweetId { get; set; }

        /// <summary>
        /// Gets or sets the UserName.
        /// </summary>
        [Required]
        public string UserName { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
