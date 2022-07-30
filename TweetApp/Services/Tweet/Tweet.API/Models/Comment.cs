namespace Tweet.API.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="Comment" />.
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Message.
        /// </summary>
        [Required]
        [MaxLength(144, ErrorMessage = "Only upto 144 characters allowed.")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the CommentAt.
        /// </summary>
        [Required]
        public DateTime CommentAt { get; set; } = DateTime.UtcNow;

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
