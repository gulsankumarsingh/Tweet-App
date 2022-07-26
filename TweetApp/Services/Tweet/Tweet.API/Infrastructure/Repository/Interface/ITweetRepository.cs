using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet.API.Models;

namespace Tweet.API.Infrastructure.Repository.Interface
{
    public interface ITweetRepository
    {
        public Task<List<TweetDetail>> GetAllTweetsAsync();
        public Task<List<TweetDetail>> GetTweetsByUserAsync(string userName);
        public Task<TweetDetail> GetTweetsByIdAsync(int tweetId);
        public Task<bool> AddTweetAsync(TweetDetail tweet);
        public Task<bool> UpdateTweetAsync(TweetDetail tweet);
        public Task<bool> DeleteTweetAsync(TweetDetail tweet);
        public Task<List<Like>> GetTweetLikesAsync(int tweetId);
        public Task<Like> GetLikesByUserNameAsync(int tweetId, string username);
        public Task<bool> LikeATweetAsync(Like tweet);
        public Task<bool> DislikeATweetAsync(Like tweet);
        public Task<List<Comment>> GetCommentsAsync(int tweetId);
        public Task<bool> AddCommentAsync(Comment comment);
        public Task<bool> SaveChangesAsync();

    }
}
