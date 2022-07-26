using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet.API.Infrastructure.Repository.Interface;
using Tweet.API.Models;
using Tweet.API.Models.Dtos;

namespace Tweet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetsController : ControllerBase
    {
        private readonly ITweetRepository _tweetRepository;
        private readonly ILogger<TweetsController> _logger;
        private readonly IMapper _mapper;

        public TweetsController(ITweetRepository tweetRepository, ILogger<TweetsController> logger, IMapper mapper)
        {
            _tweetRepository = tweetRepository ?? throw new ArgumentNullException(nameof(tweetRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

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

        [Route("{userName}/all")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TweetDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<TweetDto>>> GetTweetsByUserAsync([FromRoute]string userName)
        {
            _logger.LogInformation("GetTweetsByUserAsync method started...");
            List<TweetDetail> tweets = await _tweetRepository.GetTweetsByUserAsync(userName);
            List<TweetDto> tweetDtos = _mapper.Map<List<TweetDto>>(tweets);
            return Ok(tweetDtos);
        }

        [Route("{username}/add")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> AddTweetAsync([FromRoute]string username, [FromBody] MessageDto createTweetDto)
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
                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Tweet post Successful",
                    ResponseValue = createTweetDto
                };
                return Ok(response);
            }
            else
            {
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Something went wrong! Please try again"
                };
                return BadRequest(response);
            }
        }

        [HttpPost("/{username}/update/{tweetId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> UpdateTweetAsync([FromRoute] string username, [FromRoute] int tweetId,  [FromBody]MessageDto messageDto)
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
            else if(tweet.UserName != username)
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

            bool isTweetAdded = await _tweetRepository.UpdateTweetAsync(tweet);

            if (isTweetAdded)
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
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Something went wrong! Please try again"
                };
                return BadRequest(response);
            }
        }

        [HttpPost("{username}/delete/{tweetId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> DeleteTweetAsync([FromRoute]string username , [FromRoute]int tweetId)
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

        [Route("{username}/like/{tweetId}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> LikeTweetAsync([FromRoute] int tweetId, [FromRoute]string username)
        {
            _logger.LogInformation("LikeTweetAsync method started...");
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
            Like likeTweet = new Like
            {
                TweetId = tweetId,
                UserName = username
            };
            await _tweetRepository.LikeATweetAsync(likeTweet);
            response = new ApiResponse
            {
                Status = "Success",
                Message = "Tweet Liked"
            };
            return Ok(response);
        }

        [Route("{username}/dislike/{tweetId}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> DislikeTweetAsync([FromRoute] int tweetId, [FromRoute]string username)
        {
            _logger.LogInformation("UnlikeTweetAsync method started...");
            ApiResponse response;
            Like disLike = await _tweetRepository.GetLikesByUserNameAsync(tweetId, username);


            if (disLike == null)
            {
                _logger.LogError("No like record found");
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Tweet Not Found!"
                };
                return NotFound(response);
            }

            bool isDisliked = await _tweetRepository.DislikeATweetAsync(disLike);

            if (isDisliked)
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
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Something went wrong! Please try again"
                };
                return BadRequest(response);
            }

        }


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
                _logger.LogError("Tweet not found");
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
                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Comment Added"
                };
                return Ok(response);
            }
            else
            {
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Something went wrong! Please try again"
                };
                return BadRequest(response);
            }
            
        }

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
                _logger.LogError("Tweet not found");
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Tweet Not Found!"
                };

                return NotFound(response);
            }
            
            List<Comment> comments = await _tweetRepository.GetCommentsAsync(tweetId);
            if(comments == null)
            {
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "No comments available"
                };
                return NotFound(response);
            }

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
