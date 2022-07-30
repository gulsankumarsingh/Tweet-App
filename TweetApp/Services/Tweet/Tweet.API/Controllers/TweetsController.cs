namespace Tweet.API.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tweet.API.Infrastructure.Repository.Interface;
    using Tweet.API.Models;
    using Tweet.API.Models.Dtos;

    /// <summary>
    /// Defines the <see cref="TweetsController" />.
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    public class TweetsController : ControllerBase
    {
        /// <summary>
        /// Defines the _tweetRepository.
        /// </summary>
        private readonly ITweetRepository _tweetRepository;

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        private readonly ILogger<TweetsController> _logger;

        /// <summary>
        /// Defines the _mapper.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetsController"/> class.
        /// </summary>
        /// <param name="tweetRepository">The tweetRepository<see cref="ITweetRepository"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{TweetsController}"/>.</param>
        /// <param name="mapper">The mapper<see cref="IMapper"/>.</param>
        public TweetsController(ITweetRepository tweetRepository, ILogger<TweetsController> logger, IMapper mapper)
        {
            _tweetRepository = tweetRepository ?? throw new ArgumentNullException(nameof(tweetRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Method for Get All Tweets
        /// </summary>
        /// <returns>The All tweets.</returns>
        [Route("all")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TweetDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<TweetDto>>> GetAllTweetsAsync()
        {
            _logger.LogInformation("GetAllTweetsAsync method started...");
            List<TweetDetail> tweets = await _tweetRepository.GetAllTweetsAsync();
            List<TweetDto> tweetDtos = _mapper.Map<List<TweetDto>>(tweets);
            return Ok(tweetDtos);
        }

        /// <summary>
        /// Method for Get Tweets By User name.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The Tweet by username.</returns>
        [Route("{username}/all")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TweetDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<TweetDto>>> GetTweetsByUserAsync([FromRoute] string username)
        {
            _logger.LogInformation("GetTweetsByUserAsync method started...");
            List<TweetDetail> tweets = await _tweetRepository.GetTweetsByUserAsync(username);
            List<TweetDto> tweetDtos = _mapper.Map<List<TweetDto>>(tweets);
            return Ok(tweetDtos);
        }

        /// <summary>
        /// Method for Adding new Tweet
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="createTweetDto">Tweet message info</param>
        /// <returns>The Api response with status and message</returns>
        [Route("{username}/add")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> AddTweetAsync([FromRoute] string username, [FromBody] MessageDto createTweetDto)
        {
            _logger.LogInformation("AddTweetAsync method started...");
            ApiResponse response;
            TweetDetail tweetDetail = new TweetDetail
            {
                UserName = username,
                Message = createTweetDto.Message
            };
            bool isTweetAdded = await _tweetRepository.AddTweetAsync(tweetDetail);

            if (isTweetAdded)
            {
                _logger.LogInformation($"Tweet added successfully", tweetDetail);
                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Tweet post Successful"
                };
                return Ok(response);
            }
            else
            {
                _logger.LogInformation($"Tweet add operation failed", tweetDetail);
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Something went wrong! Please try again"
                };
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Method for Updating the Tweet
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="tweetId">The tweetId</param>
        /// <param name="messageDto">The tweet message</param>
        /// <returns>The Api response with status and message</returns>
        [Route("{username}/update/{tweetId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> UpdateTweetAsync([FromRoute] string username, [FromRoute] int tweetId, [FromBody] MessageDto messageDto)
        {
            _logger.LogInformation("UpdateTweetAsync method started...");
            ApiResponse response;

            TweetDetail tweet = await _tweetRepository.GetTweetsByIdAsync(tweetId);

            if (tweet == null)
            {
                _logger.LogError($"Tweet not found for tweet id: {tweetId}");
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Tweet Not Found!"
                };

                return NotFound(response);
            }
            else if (tweet.UserName != username)
            {
                _logger.LogError($"{username} doesn't have sufficient permission to modify the tweet", tweet);
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = $"{username} doesn't have sufficient permission to modify the tweet"
                };
                return Unauthorized(response);
            }

            tweet.Message = messageDto.Message;

            bool isTweetUpdated = await _tweetRepository.UpdateTweetAsync(tweet);

            if (isTweetUpdated)
            {
                _logger.LogInformation("Tweet updated successfully.", tweet);
                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Tweet update Successful"
                };
                return Ok(response);
            }
            else
            {
                _logger.LogError("Tweet update update operation failed", tweet);

                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Something went wrong! Please try again"
                };
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Method for Deleting a Tweet
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="tweetId">The tweetId</param>
        /// <returns>The Api response with status and message</returns>
        [Route("{username}/delete/{tweetId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> DeleteTweetAsync([FromRoute] string username, [FromRoute] int tweetId)
        {
            _logger.LogInformation("DeleteTweetAsync method started...");
            ApiResponse response;
            TweetDetail tweet = await _tweetRepository.GetTweetsByIdAsync(tweetId);

            if (tweet == null)
            {
                _logger.LogError("Tweet not found");
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Tweet Not Found!"
                };

                return NotFound(response);
            }
            else if (tweet.UserName != username)
            {
                _logger.LogError($"{username} doesn't have sufficient permission to delete the tweet", tweet);
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = $"{username} doesn't have sufficient permission to modify the tweet"
                };
                return Unauthorized(response);
            }

            bool isTweetAdded = await _tweetRepository.DeleteTweetAsync(tweet);

            if (isTweetAdded)
            {
                _logger.LogInformation($"{username} deleted tweet with tweet id : {tweet.Id}");
                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Tweet deleted Successful"
                };
                return Ok(response);
            }
            else
            {
                _logger.LogError($"{username} deleted tweet operation with tweet id : {tweet.Id} failed.");
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Something went wrong! Please try again"
                };
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Method for get the total likes count
        /// </summary>
        /// <param name="tweetId">Tweet id</param>
        /// <returns>Total likes count</returns>
        [Route("likes/count/{tweetId}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> TotalLikeOnTweetAsync([FromRoute] int tweetId)
        {
            _logger.LogInformation("TotalLikeOnTweetAsync method started...");
            int likesCount = await _tweetRepository.GetTotalLikesAsync(tweetId);

            return Ok(likesCount);

        }

        /// <summary>
        /// Method for Liking a Tweet
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="int"/>.</param>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <returns>The Api response with status and message</returns>
        [Route("{username}/like/{tweetId}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> LikeTweetAsync([FromRoute] int tweetId, [FromRoute] string username)
        {
            _logger.LogInformation("LikeTweetAsync method started...");
            ApiResponse response;
            TweetDetail tweet = await _tweetRepository.GetTweetsByIdAsync(tweetId);

            if (tweet == null)
            {
                _logger.LogError($"Tweet not found with tweet id {tweetId}");
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Tweet Not Found!"
                };

                return NotFound(response);
            }
            Like likeTweet = new Like
            {
                TweetId = tweetId,
                UserName = username
            };
            bool isTweetLiked = await _tweetRepository.LikeATweetAsync(likeTweet);
            if (isTweetLiked)
            {
                _logger.LogInformation($"Tweet liked successfully for tweet id : {tweet.Id} by user : {username}.");
                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Tweet liked successful"
                };
                return Ok(response);
            }
            else
            {
                _logger.LogError($"Tweet liking operation failed for tweet id : {tweet.Id} by user : {username}.");
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Something went wrong! Please try again"
                };
                return BadRequest(response);
            }
            
        }

        /// <summary>
        /// Method for unlike a tweet
        /// </summary>
        /// <param name="tweetId">The tweetId</param>
        /// <param name="username">The username</param>
        /// <returns>The Api response with status and message</returns>
        [Route("{username}/unlike/{tweetId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> UnlikeTweetAsync([FromRoute] int tweetId, [FromRoute] string username)
        {
            _logger.LogInformation("UnlikeTweetAsync method started...");
            ApiResponse response;
            Like disLike = await _tweetRepository.GetLikesByUserNameAsync(tweetId, username);

            if (disLike == null)
            {
                _logger.LogError($"No like record found for tweet id: {tweetId}");
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Tweet Not Found!"
                };
                return NotFound(response);
            }

            bool isDisliked = await _tweetRepository.UnlikeATweetAsync(disLike);

            if (isDisliked)
            {
                _logger.LogInformation($"Unlike operation successful for tweet id: {tweetId} by user: {username}");
                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Tweet dislike successful."
                };
                return Ok(response);
            }
            else
            {
                _logger.LogError($"Tweet unliking operation failed for tweet id : {tweetId} by user : {username}.");

                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Something went wrong! Please try again"
                };
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Method for Reply to a Tweet
        /// </summary>
        /// <param name="tweetId">The tweetId</param>
        /// <param name="username">The username</param>
        /// <param name="commentDto">The reply information</param>
        /// <returns>The Api response with status and message</returns>
        [Route("{username}/reply/{tweetId}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> ReplyToTweetAsync([FromRoute] int tweetId, [FromRoute] string username,
                [FromBody] MessageDto commentDto)
        {
            _logger.LogInformation("ReplyToTweetAsync method started...");
            ApiResponse response;
            TweetDetail tweet = await _tweetRepository.GetTweetsByIdAsync(tweetId);

            if (tweet == null)
            {
                _logger.LogError($"Tweet not found for tweet id: {tweetId} by user: {username}");
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Tweet Not Found!"
                };

                return NotFound(response);
            }
            Comment comment = new Comment
            {
                TweetId = tweetId,
                Message = commentDto.Message,
                UserName = username
            };
            bool isCommented = await _tweetRepository.AddCommentAsync(comment);
            if (isCommented)
            {
                _logger.LogInformation($"Added reply for tweet id: {tweetId} by user: {username}");
                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Comment Added"
                };
                return Ok(response);
            }
            else
            {
                _logger.LogError($"Adding reply operation failed for tweet id : {tweetId} by user : {username}.");
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Something went wrong! Please try again"
                };
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Method for Get All Reply for a tweet
        /// </summary>
        /// <param name="tweetId">The tweetId</param>
        /// <returns>The Api response with status and message</returns>
        [Route("{tweetId}/reply/all")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAllReplyAsync([FromRoute] int tweetId)
        {
            _logger.LogInformation("GetAllReplyAsync method started...");
            ApiResponse response;
            TweetDetail tweet = await _tweetRepository.GetTweetsByIdAsync(tweetId);

            if (tweet == null)
            {
                _logger.LogError($"Tweet not found for tweet id: {tweetId}");
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Tweet Not Found!"
                };

                return NotFound(response);
            }

            List<Comment> comments = await _tweetRepository.GetCommentsAsync(tweetId);
            if (comments == null)
            {
                _logger.LogInformation($"No comment found for tweet id : {tweetId}");
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "No comments available"
                };
                return NotFound(response);
            }
            else
            {
                _logger.LogInformation($"Comment found for tweet id : {tweetId}");
                List<CommentDto> commentDtos = _mapper.Map<List<CommentDto>>(comments);
                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Comment found",
                    ResponseValue = commentDtos
                };
                return Ok(response);
            }
        }
    }
}