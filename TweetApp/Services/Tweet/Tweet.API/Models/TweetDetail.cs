namespace Tweet.API.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Defines the <see cref="TweetDetail" />.
    /// </summary>
    public class TweetDetail
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
        /// Gets or sets the UpdatedAt.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the UserName.
        /// </summary>
        [Required]
        public string UserName { get; set; }
    }
}
