namespace Tweet.API.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Defines the <see cref="Reply" />.
    /// </summary>
    public class Reply
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Message.
        /// </summary>
        [Required]
        [MaxLength(144, ErrorMessage = "Only upto 144 characters allowed.")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the CreatedAt.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the CreatedAt.
        /// </summary>
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
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
    }
}
