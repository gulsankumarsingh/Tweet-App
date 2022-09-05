namespace User.API.Infrastructure.DataContext
{
    using Microsoft.Azure.Cosmos;
    using Microsoft.EntityFrameworkCore;
    using User.API.Models;

    /// <summary>
    /// Defines the <see cref="UserDbContext" />.
    /// </summary>
    public class UserDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDbContext"/> class.
        /// </summary>
        /// <param name="options">The options<see cref="DbContextOptions"/>.</param>
        public UserDbContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the UserProfiles.
        /// </summary>
        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // configuring UserProfiles
            modelBuilder.Entity<UserProfile>()
                    .ToContainer("UserProfiles") // ToContainer
                    .HasPartitionKey(e => e.Username); // Partition Key
        }
    }
}
