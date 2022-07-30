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
                tweetList = await _dbContext.TweetDetails.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while fetching all tweets!");
            }
            return tweetList;
        }

        /// <summary>
        /// The GetTweetsByIdAsync.
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="int"/>.</param>
        /// <returns>The tweet</returns>
        public async Task<TweetDetail> GetTweetsByIdAsync(int tweetId)
        {
            TweetDetail tweets = null;
            try
            {
                tweets = await _dbContext.TweetDetails.FirstOrDefaultAsync(e => e.Id == tweetId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while fetching tweets by id!");
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
                tweets = await _dbContext.TweetDetails.Where(e => e.UserName == userName).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while fetching tweets by username!");
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
                _logger.LogError(ex.Message, "An error occured while adding tweet!");
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
                _dbContext.TweetDetails.Update(tweet);
                isTweetUpdated = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while updating tweet!");
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
                _dbContext.TweetDetails.Remove(tweet);
                List<Comment> comments = await GetCommentsAsync(tweet.Id);
                List<Like> likes = await GetTweetLikesAsync(tweet.Id);
                if (comments != null)
                    _dbContext.Comments.RemoveRange(comments);

                if (likes != null)
                    _dbContext.Likes.RemoveRange(likes);

                isTweetDeleted = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while deleting tweet!");
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
                if(tweetDetails != null && tweetDetails.Count > 0)
                {
                    foreach (var tweetId in tweetDetails.Select(i => i.Id))
                    {
                        List<Comment> comments = await GetCommentsAsync(tweetId);
                        List<Like> likes = await GetTweetLikesAsync(tweetId);
                        if (comments != null)
                            _dbContext.Comments.RemoveRange(comments);

                        if (likes != null)
                            _dbContext.Likes.RemoveRange(likes);
                    }
                    _dbContext.TweetDetails.RemoveRange(tweetDetails);
                    isTweetDeleted = await SaveChangesAsync();
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"An error occured while deleting tweet for user: {username}!");
            }
            return isTweetDeleted;
        }

        /// <summary>
        /// The GetCommentsAsync.
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="int"/>.</param>
        /// <returns>The list of comments</returns>
        public async Task<List<Comment>> GetCommentsAsync(int tweetId)
        {
            List<Comment> comments = null;
            try
            {
                comments = await _dbContext.Comments.Where(e => e.TweetId == tweetId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while fetching comments by tweet id!");
            }
            return comments;
        }

        /// <summary>
        /// The AddCommentAsync.
        /// </summary>
        /// <param name="comment">The comment<see cref="Comment"/>.</param>
        /// <returns>true if comment added else false</returns>
        public async Task<bool> AddCommentAsync(Comment comment)
        {
            bool isCommentAdded = false;
            try
            {
                await _dbContext.Comments.AddAsync(comment);
                isCommentAdded = await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while adding comment!");
            }
            return isCommentAdded;
        }

        /// <summary>
        /// The GetTweetLikesAsync.
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="int"/>.</param>
        /// <returns>The list of likes</returns>
        public async Task<List<Like>> GetTweetLikesAsync(int tweetId)
        {
            List<Like> likes = null;
            try
            {
                likes = await _dbContext.Likes.Where(e => e.TweetId == tweetId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while fetching likes by tweet id!");
            }
            return likes;
        }

        /// <summary>
        /// The GetTotalLikesAsync.
        /// </summary>
        /// <param name="tweetId">The tweet id.</param>
        /// <returns>Total like count.</returns>
        public async Task<int> GetTotalLikesAsync(int tweetId)
        {
            int likesCount = 0;
            try
            {
                likesCount = await _dbContext.Likes.CountAsync(e => e.TweetId == tweetId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while fetching likes count tweet id!");
            }
            return likesCount;
        }

        /// <summary>
        /// The GetLikesByUserNameAsync.
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="int"/>.</param>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <returns>The like detail/>.</returns>
        public async Task<Like> GetLikesByUserNameAsync(int tweetId, string username)
        {
            Like likeInfo = null;
            try
            {
                likeInfo = await _dbContext.Likes.FirstOrDefaultAsync(e => e.TweetId == tweetId && e.UserName == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while fetching likes by user name!");
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
                _logger.LogError(ex.Message, "An error occured while liking the tweet!");
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
                _logger.LogError(ex.Message, "An error occured while disliking the tweet!");
            }
            return isUnliked;
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
                _logger.LogError(ex.Message, "An error occured while saving the record to db!");
            }
            return isRecordSaved;
        }
    }
}
