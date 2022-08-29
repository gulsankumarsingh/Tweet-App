namespace Tweet.API.Infrastructure.Repository
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Tweet.API.Infrastructure.DataContext;
    using Tweet.API.Infrastructure.Repository.Interface;
    using Tweet.API.Models;

    /// <summary>
    /// Defines the <see cref="TweetRepository" />.
    /// </summary>
    public class TweetRepository : ITweetRepository
    {
        /// <summary>
        /// Defines the _dbContext.
        /// </summary>
        private readonly TweetDbContext _dbContext;

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        private readonly ILogger<TweetRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext<see cref="TweetDbContext"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{TweetRepository}"/>.</param>
        public TweetRepository(TweetDbContext dbContext, ILogger<TweetRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The GetAllTweetsAsync.
        /// </summary>
        /// <returns>The list of tweets</returns>
        public async Task<List<TweetDetail>> GetAllTweetsAsync()
        {
            List<TweetDetail> tweetList = null;
            try
            {
                tweetList = await _dbContext.TweetDetails.AsNoTracking().OrderByDescending(i => i.UpdatedAt).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while fetching all tweets!", ex.Message);
            }
            return tweetList;
        }

        /// <summary>
        /// The GetTweetsByIdAsync.
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="string"/>.</param>
        /// <returns>The tweet</returns>
        public async Task<TweetDetail> GetTweetsByIdAsync(string tweetId)
        {
            TweetDetail tweets = null;
            try
            {
                tweets = await _dbContext.TweetDetails.AsNoTracking().FirstOrDefaultAsync(e => e.Id == tweetId);
            }
            catch (Exception ex)
            {
                _logger.LogError( "An error occured while fetching tweets by id!", ex.Message);
            }
            return tweets;
        }

        /// <summary>
        /// The GetTweetsByUserAsync.
        /// </summary>
        /// <param name="userName">The userName<see cref="string"/>.</param>
        /// <returns>The list of tweets</returns>
        public async Task<List<TweetDetail>> GetTweetsByUserAsync(string userName)
        {
            List<TweetDetail> tweets = null;
            try
            {
                tweets = await _dbContext.TweetDetails.AsNoTracking().Where(e => e.UserName == userName).OrderByDescending(u => u.UpdatedAt).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while fetching tweets by username!", ex.Message);
            }
            return tweets;
        }

        /// <summary>
        /// The AddTweetAsync.
        /// </summary>
        /// <param name="tweet">The tweet<see cref="TweetDetail"/>.</param>
        /// <returns>If tweet added then true else false</returns>
        public async Task<bool> AddTweetAsync(TweetDetail tweet)
        {
            bool isTweetAdded = false;
            try
            {
                await _dbContext.TweetDetails.AddAsync(tweet);
                isTweetAdded = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while adding tweet!", ex.Message);
            }
            return isTweetAdded;
        }

        /// <summary>
        /// The UpdateTweetAsync.
        /// </summary>
        /// <param name="tweet">The tweet<see cref="TweetDetail"/>.</param>
        /// <returns>The If tweet updated then true else false</returns>
        public async Task<bool> UpdateTweetAsync(TweetDetail tweet)
        {
            bool isTweetUpdated = false;
            try
            {
                //_dbContext.TweetDetails.Update(tweet);
                //isTweetUpdated = await SaveChangesAsync();
                var entry = _dbContext.TweetDetails.Add(tweet);
                entry.State = EntityState.Unchanged;
                _dbContext.Update(tweet);
                isTweetUpdated = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while updating tweet!", ex.Message);
            }
            return isTweetUpdated;
        }

        /// <summary>
        /// The DeleteTweetAsync.
        /// </summary>
        /// <param name="tweet">The tweet<see cref="TweetDetail"/>.</param>
        /// <returns>If tweet deleted then true else false.</returns>
        public async Task<bool> DeleteTweetAsync(TweetDetail tweet)
        {
            bool isTweetDeleted = false;
            try
            {
                var entry = _dbContext.TweetDetails.Add(tweet);
                entry.State = EntityState.Unchanged;
                _dbContext.TweetDetails.Remove(tweet);
                List<Reply> comments = await GetAllReplyByTweetIdAsync(tweet.Id);
                List<Like> likes = await GetTweetLikesAsync(tweet.Id);
                if (comments.Any())
                    _dbContext.Replies.RemoveRange(comments);

                if (likes.Any())
                    _dbContext.Likes.RemoveRange(likes);

                isTweetDeleted = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while deleting tweet!", ex.Message);
            }
            return isTweetDeleted;
        }

        /// <summary>
        /// The DeleteTweetAsync.
        /// </summary>
        /// <param name="username">The user name <see cref="string"/>.</param>
        /// <returns>If tweet deleted then true else false.</returns>
        public async Task<bool> DeleteTweetByUserNameAsync(string username)
        {
            bool isTweetDeleted = false;
            try
            {
                List<TweetDetail> tweetDetails = await GetTweetsByUserAsync(username);
                if(tweetDetails.Any())
                {
                    for(int i = 0; i < tweetDetails.Count; i++)
                    {
                        isTweetDeleted = await DeleteTweetAsync(tweetDetails[i]);   
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while deleting tweet for user: {username}!", ex.Message);
            }
            return isTweetDeleted;
        }
        /// <summary>
        /// The GetReplyByIdAsync.
        /// </summary>
        /// <param name="id">The tweetId<see cref="string"/>.</param>
        /// <returns>The reply</returns>
        public async Task<Reply> GetReplyByIdAsync(string id)
        {
            Reply reply = null;
            try
            {
                reply = await _dbContext.Replies.FirstOrDefaultAsync(i => i.Id == id);
            }
            catch (Exception ex)
            {

                _logger.LogError( "An error occured while fetching reply by id!", ex.Message);
            }
            return reply;
        }
        /// <summary>
        /// The GetAllReplyByTweetIdAsync.
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="string"/>.</param>
        /// <returns>The list of comments</returns>
        public async Task<List<Reply>> GetAllReplyByTweetIdAsync(string tweetId)
        {
            List<Reply> replies = null;
            try
            {
                replies = await _dbContext.Replies.Where(e => e.TweetId == tweetId).OrderByDescending(i => i.UpdatedAt).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while fetching replies by tweet id!", ex.Message);
            }
            return replies;
        }

        /// <summary>
        /// The AddReplyAsync.
        /// </summary>
        /// <param name="reply">The reply<see cref="Reply"/>.</param>
        /// <returns>true if comment added else false</returns>
        public async Task<bool> AddReplyAsync(Reply reply)
        {
            bool isReplyAdded = false;
            try
            {
                await _dbContext.Replies.AddAsync(reply);
                isReplyAdded = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while adding reply!", ex.Message);
            }
            return isReplyAdded;
        }

        /// <summary>
        /// The AddReplyAsync.
        /// </summary>
        /// <param name="reply">The reply<see cref="Reply"/>.</param>
        /// <returns>true if comment added else false</returns>
        public async Task<bool> UpdateReplyAsync(Reply reply)
        {
            bool isReplyAdded = false;
            try
            {
                _dbContext.Replies.Update(reply);
                isReplyAdded = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while updating reply!", ex.Message);
            }
            return isReplyAdded;
        }

        /// <summary>
        /// The AddReplyAsync.
        /// </summary>
        /// <param name="reply">The reply<see cref="Reply"/>.</param>
        /// <returns>true if comment added else false</returns>
        public async Task<bool> DeleteReplyAsync(Reply reply)
        {
            bool isReplyDeleted = false;
            try
            {
                _dbContext.Replies.Remove(reply);
                isReplyDeleted = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while removing reply!", ex.Message);
            }
            return isReplyDeleted;
        }

        /// <summary>
        /// The DeleteReplyAsync.
        /// </summary>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <returns>true if reply deleted else false</returns>
        public async Task<bool> DeleteReplyByUsernameAsync(string username)
        {
            bool isReplyDeleted = false;
            try
            {
                List<Reply> replies = await _dbContext.Replies.Where(i => i.UserName == username).ToListAsync();
                if (replies.Any())
                {
                    _dbContext.Replies.RemoveRange(replies);
                    isReplyDeleted = await SaveChangesAsync();
                }
                else
                {
                    isReplyDeleted = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while removing all reply by username {username}!", ex.Message);
            }
            return isReplyDeleted;
        }
        /// <summary>
        /// The GetTweetLikesAsync.
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="string"/>.</param>
        /// <returns>The list of likes</returns>
        public async Task<List<Like>> GetTweetLikesAsync(string tweetId)
        {
            List<Like> likes = null;
            try
            {
                likes = await _dbContext.Likes.Where(e => e.TweetId == tweetId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while fetching likes by tweet id!", ex.Message);
            }
            return likes;
        }

        /// <summary>
        /// The GetLikesByUserNameAsync.
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="string"/>.</param>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <returns>The like detail/>.</returns>
        public async Task<Like> GetLikesByUserNameAsync(string tweetId, string username)
        {
            Like likeInfo = null;
            try
            {
                likeInfo = await _dbContext.Likes.FirstOrDefaultAsync(e => e.TweetId == tweetId && e.UserName == username);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while fetching likes by user name!", ex.Message);
            }
            return likeInfo;
        }

        /// <summary>
        /// The LikeATweetAsync.
        /// </summary>
        /// <param name="tweet">The tweet<see cref="Like"/>.</param>
        /// <returns>true if like successful else false</returns>
        public async Task<bool> LikeATweetAsync(Like tweet)
        {
            bool isLikeAdded = false;
            try
            {
                await _dbContext.Likes.AddAsync(tweet);
                isLikeAdded = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while liking the tweet!", ex.Message);
            }
            return isLikeAdded;
        }

        /// <summary>
        /// The UnlikeATweetAsync.
        /// </summary>
        /// <param name="tweet">The tweet<see cref="Like"/>.</param>
        /// <returns>true if unlike successful else false</returns>
        public async Task<bool> UnlikeATweetAsync(Like tweet)
        {
            bool isUnliked = false;
            try
            {
                _dbContext.Likes.Remove(tweet);
                isUnliked = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while disliking the tweet!", ex.Message);
            }
            return isUnliked;
        }

        /// <summary>
        /// The UnlikeATweetAsync.
        /// </summary>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <returns>The true if unliked else false</returns>
        public async Task<bool> DeleteLikeByUsernameAsync(string username)
        {
            bool isLikeDeleted = false;
            try
            {
                List<Like> likes = await _dbContext.Likes.Where(i => i.UserName == username).ToListAsync();
                if (likes.Any())
                {
                    _dbContext.Likes.RemoveRange(likes);
                    isLikeDeleted = await SaveChangesAsync();
                }
                else
                {
                    isLikeDeleted = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while removing all likes by username {username}!", ex.Message);
            }
            return isLikeDeleted;
        }

        /// <summary>
        /// The SaveChangesAsync.
        /// </summary>
        /// <returns>True if records saved in database else false</returns>
        public async Task<bool> SaveChangesAsync()
        {
            bool isRecordSaved = false;
            try
            {
                isRecordSaved = await _dbContext.SaveChangesAsync() >= 0;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured while saving the record to db!", ex.Message);
            }
            return isRecordSaved;
        }
    }
}
