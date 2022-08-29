namespace Tweet.API.Infrastructure.Repository.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tweet.API.Models;

    /// <summary>
    /// Defines the <see cref="ITweetRepository" />.
    /// </summary>
    public interface ITweetRepository
    {
        /// <summary>
        /// The GetAllTweetsAsync.
        /// </summary>
        /// <returns>The list of tweets</returns>
        public Task<List<TweetDetail>> GetAllTweetsAsync();

        /// <summary>
        /// The GetTweetsByUserAsync.
        /// </summary>
        /// <param name="userName">The userName<see cref="string"/>.</param>
        /// <returns>The list of tweets</returns>
        public Task<List<TweetDetail>> GetTweetsByUserAsync(string userName);

        /// <summary>
        /// The GetTweetsByIdAsync.
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="int"/>.</param>
        /// <returns>The tweet</returns>
        public Task<TweetDetail> GetTweetsByIdAsync(string tweetId);

        /// <summary>
        /// The AddTweetAsync.
        /// </summary>
        /// <param name="tweet">The tweet<see cref="TweetDetail"/>.</param>
        /// <returns>If tweet added then true else false</returns>
        public Task<bool> AddTweetAsync(TweetDetail tweet);

        /// <summary>
        /// The UpdateTweetAsync.
        /// </summary>
        /// <param name="tweet">The tweet<see cref="TweetDetail"/>.</param>
        /// <returns>The If tweet updated then true else false</returns>
        public Task<bool> UpdateTweetAsync(TweetDetail tweet);

        /// <summary>
        /// The DeleteTweetAsync.
        /// </summary>
        /// <param name="tweet">The tweet<see cref="TweetDetail"/>.</param>
        /// <returns>If tweet deleted then true else false.</returns>
        public Task<bool> DeleteTweetAsync(TweetDetail tweet);

        /// <summary>
        /// The DeleteTweetByUserNameAsync.
        /// </summary>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <returns>If tweet deleted then true else false.</returns>
        public Task<bool> DeleteTweetByUserNameAsync(string username);

        /// <summary>
        /// The GetTweetLikesAsync.
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="int"/>.</param>
        /// <returns>the list of like detail</returns>
        public Task<List<Like>> GetTweetLikesAsync(string tweetId);

        /// <summary>
        /// The GetLikesByUserNameAsync.
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="int"/>.</param>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <returns>the like detail</returns>
        public Task<Like> GetLikesByUserNameAsync(string tweetId, string username);

        /// <summary>
        /// The LikeATweetAsync.
        /// </summary>
        /// <param name="tweet">The tweet<see cref="Like"/>.</param>
        /// <returns>The true if liked else false</returns>
        public Task<bool> LikeATweetAsync(Like tweet);

        /// <summary>
        /// The UnlikeATweetAsync.
        /// </summary>
        /// <param name="tweet">The tweet<see cref="Like"/>.</param>
        /// <returns>The true if unliked else false</returns>
        public Task<bool> UnlikeATweetAsync(Like tweet);

        /// <summary>
        /// The UnlikeATweetAsync.
        /// </summary>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <returns>The true if unliked else false</returns>
        public Task<bool> DeleteLikeByUsernameAsync(string username);

        /// <summary>
        /// The GetAllReplyByTweetIdAsync.
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="int"/>.</param>
        /// <returns>The list of comments</returns>
        public Task<List<Reply>> GetAllReplyByTweetIdAsync(string tweetId);
        /// <summary>
        /// The GetReplyByIdAsync.
        /// </summary>
        /// <param name="id">The tweetId<see cref="int"/>.</param>
        /// <returns>The list of comments</returns>
        public Task<Reply> GetReplyByIdAsync(string id);

        /// <summary>
        /// The AddReplyAsync.
        /// </summary>
        /// <param name="reply">The reply<see cref="Reply"/>.</param>
        /// <returns>true if comment added else false</returns>
        public Task<bool> AddReplyAsync(Reply reply);
        /// <summary>
        /// The UpdateReplyAsync.
        /// </summary>
        /// <param name="reply">The reply<see cref="Reply"/>.</param>
        /// <returns>true if comment added else false</returns>
        Task<bool> UpdateReplyAsync(Reply reply);
        /// <summary>
        /// The DeleteReplyAsync.
        /// </summary>
        /// <param name="reply">The reply<see cref="Reply"/>.</param>
        /// <returns>true if comment added else false</returns>
        Task<bool> DeleteReplyAsync(Reply reply);

        /// <summary>
        /// The DeleteReplyAsync.
        /// </summary>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <returns>true if comment added else false</returns>
        Task<bool> DeleteReplyByUsernameAsync(string username);

        /// <summary>
        /// The SaveChangesAsync.
        /// </summary>
        /// <returns>True if records saved in database else false</returns>
        public Task<bool> SaveChangesAsync();
    }
}
