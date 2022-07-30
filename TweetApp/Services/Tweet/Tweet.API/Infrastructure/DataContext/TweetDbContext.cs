namespace Tweet.API.Infrastructure.DataContext
{
    using Microsoft.EntityFrameworkCore;
    using Tweet.API.Models;

    /// <summary>
    /// Defines the <see cref="TweetDbContext" />.
    /// </summary>
    public class TweetDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TweetDbContext"/> class.
        /// </summary>
        /// <param name="options">The options<see cref="DbContextOptions"/>.</param>
        public TweetDbContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the TweetDetails.
        /// </summary>
        public DbSet<TweetDetail> TweetDetails { get; set; }

        /// <summary>
        /// Gets or sets the Comments.
        /// </summary>
        public DbSet<Comment> Comments { get; set; }

        /// <summary>
        /// Gets or sets the Likes.
        /// </summary>
        public DbSet<Like> Likes { get; set; }
    }
}
