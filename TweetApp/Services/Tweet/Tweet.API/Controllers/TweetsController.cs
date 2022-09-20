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
    using Tweet.API.Infrastructure.ActionResults;
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
        /// Get All Tweets
        /// </summary>
        /// <returns>The All tweets.</returns>
        [Route("all")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TweetInfoDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<TweetInfoDto>>> GetAllTweetsAsync()
        {
            try
            {
                _logger.LogInformation("GetAllTweetsAsync method started...");
                List<TweetDetail> tweets = await _tweetRepository.GetAllTweetsAsync();
                List<TweetDto> tweetDtos = _mapper.Map<List<TweetDto>>(tweets);
                List<TweetInfoDto> tweetInfoDtos = new List<TweetInfoDto>();
                foreach (TweetDto tweet in tweetDtos)
                {
                    tweetInfoDtos.Add(new TweetInfoDto()
                    {
                        Tweet = tweet,
                        Likes = _mapper.Map<List<LikeDto>>(await _tweetRepository.GetTweetLikesAsync(tweet.Id)),
                        Comments = _mapper.Map<List<ReplyDto>>(await _tweetRepository.GetAllReplyByTweetIdAsync(tweet.Id))
                    });
                }
                return Ok(tweetInfoDtos);
            }
            catch (Exception ex)
            {

                _logger.LogError($"An error occured while getting all the tweets", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }

        /// <summary>
        /// Get Tweets By id.
        /// </summary>
        /// <param name="tweetId">The tweetId.</param>
        /// <returns>The Tweet by tweet Id.</returns>
        [Route("get/{tweetId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TweetInfoDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TweetInfoDto>> GetTweetByTweetIdAsync([FromRoute] string tweetId)
        {
            try
            {
                _logger.LogInformation("GetTweetByTweetIdAsync method started...");
                TweetDetail tweet = await _tweetRepository.GetTweetsByIdAsync(tweetId);
                if (tweet == null)
                {
                    return NotFound(new ApiResponse { Status = "Error", Message = "Tweet not found" });
                }
                TweetInfoDto tweetInfo = new TweetInfoDto
                {
                    Tweet = _mapper.Map<TweetDto>(tweet),
                    Likes = _mapper.Map<List<LikeDto>>(await _tweetRepository.GetTweetLikesAsync(tweet.Id)),
                    Comments = _mapper.Map<List<ReplyDto>>(await _tweetRepository.GetAllReplyByTweetIdAsync(tweet.Id))
                };
                return Ok(tweetInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while getting tweet by id: {tweetId}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
           
        }

        /// <summary>
        /// Get Tweets By User name.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The Tweet by username.</returns>
        [Route("{username}/all")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TweetInfoDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<TweetInfoDto>>> GetTweetsByUserAsync([FromRoute] string username)
        {
            try
            {
                _logger.LogInformation("GetTweetsByUserAsync method started...");
                List<TweetDetail> tweets = await _tweetRepository.GetTweetsByUserAsync(username);
                List<TweetDto> tweetDtos = _mapper.Map<List<TweetDto>>(tweets);
                List<TweetInfoDto> tweetInfoDtos = new List<TweetInfoDto>();
                foreach (TweetDto tweet in tweetDtos)
                {
                    tweetInfoDtos.Add(new TweetInfoDto()
                    {
                        Tweet = tweet,
                        Likes = _mapper.Map<List<LikeDto>>(await _tweetRepository.GetTweetLikesAsync(tweet.Id)),
                        Comments = _mapper.Map<List<ReplyDto>>(await _tweetRepository.GetAllReplyByTweetIdAsync(tweet.Id))
                    });
                }
                return Ok(tweetInfoDtos);
            }
            catch (Exception ex)
            {

                _logger.LogError($"An error occured while getting tweet by username : {username}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }

        /// <summary>
        /// Add a new Tweet
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
            try
            {
                _logger.LogInformation("AddTweetAsync method started...");
                ApiResponse response;
                TweetDetail tweetDetail = new TweetDetail
                {
                    UserName = username,
                    Message = createTweetDto.Message
                };
                
                bool isSuccess = await _tweetRepository.AddTweetAsync(tweetDetail);
                if (isSuccess)
                {
                    response = new ApiResponse
                    {
                        Status = "Success",
                        Message = "Tweet post Successful"
                    };
                    return Ok(response);
                }
                else
                {
                    return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while adding the tweet by user: {username}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }

        /// <summary>
        /// Update a Tweet
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
        public async Task<ActionResult<ApiResponse>> UpdateTweetAsync([FromRoute] string username, [FromRoute] string tweetId, [FromBody] MessageDto messageDto)
        {
            try
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
                tweet.UpdatedAt = DateTime.Now;

                bool isSuccess = await _tweetRepository.UpdateTweetAsync(tweet);

                if (isSuccess)
                {
                    response = new ApiResponse
                    {
                        Status = "Success",
                        Message = "Tweet update Successful"
                    };
                    return Ok(response);
                }
                else
                {
                    return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while updating the tweet id: {tweetId} by user: {username}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }

        /// <summary>
        /// Delete a Tweet
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
        public async Task<ActionResult<ApiResponse>> DeleteTweetAsync([FromRoute] string username, [FromRoute] string tweetId)
        {
            try
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

                bool isSuccess = await _tweetRepository.DeleteTweetAsync(tweet);
                
                if (isSuccess)
                {
                    response = new ApiResponse
                    {
                        Status = "Success",
                        Message = "Tweet deleted Successful"
                    };
                    return Ok(response);
                }
                else
                {
                    return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while deleting the tweet id: {tweetId} by user: {username}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
        }

        /// <summary>
        /// Like a Tweet
        /// </summary>
        /// <param name="tweetId">The tweetId<see cref="string"/>.</param>
        /// <param name="username">The username<see cref="string"/>.</param>
        /// <returns>The Api response with status and message</returns>
        [Route("{username}/like/{tweetId}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> LikeTweetAsync([FromRoute] string tweetId, [FromRoute] string username)
        {
            try
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
                bool isSuccess = await _tweetRepository.LikeATweetAsync(likeTweet);

                if (isSuccess)
                {
                    response = new ApiResponse
                    {
                        Status = "Success",
                        Message = "Tweet liked successful"
                    };
                    return Ok(response);
                }
                else
                {
                    return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while liking the tweet id: {tweetId} by user: {username} ", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
            
        }

        /// <summary>
        /// Unlike a tweet
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
        public async Task<ActionResult<ApiResponse>> UnlikeTweetAsync([FromRoute] string tweetId, [FromRoute] string username)
        {
            try
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

                bool isSuccess = await _tweetRepository.UnlikeATweetAsync(disLike);

                if (isSuccess)
                {
                    response = new ApiResponse
                    {
                        Status = "Success",
                        Message = "Tweet dislike successful."
                    };
                    return Ok(response);
                }
                else
                {
                    return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while unliking the tweet id: {tweetId} by user: {username} ", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }

        }

        /// <summary>
        /// Add reply
        /// </summary>
        /// <param name="tweetId">The tweetId</param>
        /// <param name="username">The username</param>
        /// <param name="messgaeDto">The reply information</param>
        /// <returns>The Api response with status and message</returns>
        [Route("{username}/reply/{tweetId}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> ReplyToTweetAsync([FromRoute] string tweetId, [FromRoute] string username,
                [FromBody] MessageDto messgaeDto)
        {
            try
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
                Reply comment = new Reply
                {
                    TweetId = tweetId,
                    Message = messgaeDto.Message,
                    UserName = username
                };
                bool isSuccess = await _tweetRepository.AddReplyAsync(comment);

                if (isSuccess)
                {
                    response = new ApiResponse
                    {
                        Status = "Success",
                        Message = "Comment Added"
                    };
                    return Ok(response);
                }
                else
                {
                    return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while adding the reply for the tweet id: {tweetId} by user: {username}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }

        /// <summary>
        /// Updating reply
        /// </summary>
        /// <param name="replyId">The replyId</param>
        /// <param name="username">The username</param>
        /// <param name="messageDto">The reply information</param>
        /// <returns>The Api response with status and message</returns>
        [Route("{username}/reply/update/{replyId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> UpdateReplyToTweetAsync([FromRoute] string replyId, [FromRoute] string username,
                [FromBody] MessageDto messageDto)
        {
            try
            {
                _logger.LogInformation("UpdateReplyToTweetAsync method started...");
                ApiResponse response;
                Reply reply = await _tweetRepository.GetReplyByIdAsync(replyId);

                if (reply == null)
                {
                    _logger.LogError($"Reply not found for reply id: {replyId} by user: {username}");
                    response = new ApiResponse
                    {
                        Status = "Error",
                        Message = "Reply Not Found!"
                    };

                    return NotFound(response);
                }
                reply.Message = messageDto.Message;
                reply.UpdatedAt = DateTime.Now;
                bool isSuccess = await _tweetRepository.UpdateReplyAsync(reply);

                if (isSuccess)
                {
                    response = new ApiResponse
                    {
                        Status = "Success",
                        Message = "Reply Updated"
                    };
                    return Ok(response);
                }
                else
                {
                    return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while updating the reply for reply id: {replyId} by user: {username} ", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }

        /// <summary>
        /// Deleting reply
        /// </summary>
        /// <param name="replyId">The replyId</param>
        /// <param name="username">The username</param>
        /// <returns>The Api response with status and message</returns>
        [Route("{username}/reply/delete/{replyId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> DeleteReplyToTweetAsync([FromRoute] string replyId, [FromRoute] string username)
        {
            try
            {
                _logger.LogInformation("DeleteReplyToTweetAsync method started...");
                ApiResponse response;
                Reply reply = await _tweetRepository.GetReplyByIdAsync(replyId);

                if (reply == null)
                {
                    _logger.LogError($"Reply not found for reply id: {replyId} by user: {username}");
                    response = new ApiResponse
                    {
                        Status = "Error",
                        Message = "Reply Not Found!"
                    };

                    return NotFound(response);
                }
                bool isSuccess = await _tweetRepository.DeleteReplyAsync(reply);

                if (isSuccess)
                {
                    response = new ApiResponse
                    {
                        Status = "Success",
                        Message = "Reply Deleted"
                    };
                    return Ok(response);
                }
                else
                {
                    return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while deleting the reply for reply id: {replyId} by user: {username} ", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }

        /// <summary>
        /// Get All Reply for a tweet
        /// </summary>
        /// <param name="tweetId">The tweetId</param>
        /// <returns>The Api response with status and message</returns>
        [Route("{tweetId}/reply/all")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetAllReplyAsync([FromRoute] string tweetId)
        {
            try
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

                List<Reply> comments = await _tweetRepository.GetAllReplyByTweetIdAsync(tweetId);
                if (comments.Count == 0)
                {
                    _logger.LogInformation($"No comment found for tweet id : {tweetId}");
                    response = new ApiResponse
                    {
                        Status = "Success",
                        Message = "No comments available"
                    };
                    return Ok(response);
                }
                else
                {
                    _logger.LogInformation($"Comment found for tweet id : {tweetId}");
                    List<ReplyDto> commentDtos = _mapper.Map<List<ReplyDto>>(comments);
                    response = new ApiResponse
                    {
                        Status = "Success",
                        Message = "Comment found",
                        ResponseValue = commentDtos
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while getting all the reply by tweet id : {tweetId}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }
    }
}