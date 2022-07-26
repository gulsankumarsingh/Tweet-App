using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet.API.Models;

namespace Tweet.API.Infrastructure.DataContext
{
    public class TweetDbContext : DbContext
    {
        public TweetDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TweetDetail> TweetDetails { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
    }
}
