namespace Tweet.API.Models.Dtos
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="LikeDto" />.
    /// </summary>
    public class LikeDto
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public string Id { get; set; }

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
