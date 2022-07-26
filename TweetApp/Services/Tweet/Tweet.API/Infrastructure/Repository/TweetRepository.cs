using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet.API.Infrastructure.DataContext;
using Tweet.API.Infrastructure.Repository.Interface;
using Tweet.API.Models;

namespace Tweet.API.Infrastructure.Repository
{
    public class TweetRepository : ITweetRepository
    {

        private readonly TweetDbContext _dbContext;
        private readonly ILogger<TweetRepository> _logger;

        public TweetRepository(TweetDbContext dbContext, ILogger<TweetRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Tweets
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


        public async Task<bool> AddTweetAsync(TweetDetail tweet)
        {
            bool isTweetAdded = false;
            try
            {
                await _dbContext.TweetDetails.AddAsync(tweet);
                isTweetAdded = await SaveChangesAsync();
                _logger.LogInformation("Tweet added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while adding tweet!");
            }
            return isTweetAdded;
        }

        public async Task<bool> UpdateTweetAsync(TweetDetail tweet)
        {
            bool isTweetUpdated = false;
            try
            {
                _dbContext.TweetDetails.Update(tweet);
                isTweetUpdated = await SaveChangesAsync();
                _logger.LogInformation("Tweet updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while updating tweet!");
            }
            return isTweetUpdated;
        }

        public async Task<bool> DeleteTweetAsync(TweetDetail tweet)
        {
            bool isTweetDeleted = false;
            try
            {
                _dbContext.TweetDetails.Remove(tweet);
                List<Comment> comments = await GetCommentsAsync(tweet.Id);
                List<Like> likes = await GetTweetLikesAsync(tweet.Id);
                if(comments != null)
                    _dbContext.Comments.RemoveRange(comments);

                if(likes != null)
                    _dbContext.Likes.RemoveRange(likes);

                isTweetDeleted = await SaveChangesAsync();
                _logger.LogInformation("Tweet deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while deleting tweet!");
            }
            return isTweetDeleted;
        }


        #endregion

        #region Comment on Tweet
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

        public async Task<bool> AddCommentAsync(Comment comment)
        {
            bool isCommentAdded = false;
            try
            {
                await _dbContext.Comments.AddAsync(comment);
                isCommentAdded = await SaveChangesAsync();
                _logger.LogInformation("Comment added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while adding comment!");
            }
            return isCommentAdded;
        }
        #endregion

        #region Like a Tweet
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

        public async Task<bool> LikeATweetAsync(Like tweet)
        {
            bool isLikeAdded = false;
            try
            {
                await _dbContext.Likes.AddAsync(tweet);
                isLikeAdded = await SaveChangesAsync();
                _logger.LogInformation("Tweet liked successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while liking the tweet!");
            }
            return isLikeAdded;
        }

        public async Task<bool> DislikeATweetAsync(Like tweet)
        {
            bool isUnliked = false;
            try
            {
                _dbContext.Likes.Remove(tweet);
                isUnliked = await SaveChangesAsync();
                _logger.LogInformation("Tweet disliked successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occured while disliking the tweet!");
            }
            return isUnliked;
        }

        #endregion

        #region Common
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
        #endregion

    }
}
