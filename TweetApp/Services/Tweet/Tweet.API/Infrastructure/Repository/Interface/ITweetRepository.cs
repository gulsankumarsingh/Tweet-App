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
        public Task<TweetDetail> GetTweetsByIdAsync(int tweetId);

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
        public Task<List<Like>> GetTweetLikesAsync(int tweetId);

        /// <summary>
        /// The GetTotalLikesAsync
        /// </summary>
        /// <param name="tweetId">The tweet id</param>
        /// <returns>Total like count</returns>
        public Task<int> GetTotalLikesAsync(int tweetId);

        /// <summary>
        /// The GetLikesByUserNameAsync.
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="int"/>.</param>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <returns>the like detail</returns>
        public Task<Like> GetLikesByUserNameAsync(int tweetId, string username);

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
        /// The GetCommentsAsync.
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="int"/>.</param>
        /// <returns>The list of comments</returns>
        public Task<List<Comment>> GetCommentsAsync(int tweetId);

        /// <summary>
        /// The AddCommentAsync.
        /// </summary>
        /// <param name="comment">The comment<see cref="Comment"/>.</param>
        /// <returns>true if comment added else false</returns>
        public Task<bool> AddCommentAsync(Comment comment);

        /// <summary>
        /// The SaveChangesAsync.
        /// </summary>
        /// <returns>True if records saved in database else false</returns>
        public Task<bool> SaveChangesAsync();
    }
}
