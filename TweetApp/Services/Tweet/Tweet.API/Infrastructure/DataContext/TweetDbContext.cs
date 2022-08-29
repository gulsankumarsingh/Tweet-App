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
        /// Gets or sets the Replies.
        /// </summary>
        public DbSet<Reply> Replies { get; set; }

        /// <summary>
        /// Gets or sets the Likes.
        /// </summary>
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // configuring TweetDetails
            modelBuilder.Entity<TweetDetail>()
                    .ToContainer("TweetDetails") // ToContainer
                    .HasPartitionKey(e => e.Id); // Partition Key

            // configuring Replies
            modelBuilder.Entity<Reply>()
                    .ToContainer("Replies") // ToContainer
                    .HasPartitionKey(e => e.Id); // Partition Key

            // configuring Likes
            modelBuilder.Entity<Like>()
                    .ToContainer("Likes") // ToContainer
                    .HasPartitionKey(e => e.Id); // Partition Key
        }
    }
}
