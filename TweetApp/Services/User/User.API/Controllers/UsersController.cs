using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.API.Infrastructure.Repository.Interface;
using User.API.Models;
using User.API.Models.Dtos;

namespace UserService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [Route("all")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            _logger.LogInformation("GetAllUsers method started...");
            List<UserProfile> userProfiles = await _userRepository.GetAllUsersAsync();
            List<UserDto> userProfileDtos = _mapper.Map<List<UserDto>>(userProfiles);
            return Ok(userProfileDtos);
        }

        [Route("get")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetUserAsync(string userName)
        {
            _logger.LogInformation("GetUserAsync method started...");
            ApiResponse response;
            UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(userName);

            if (userProfile == null)
            {
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "User not found!"
                };

                return NotFound(response);
            }

            UserProfileDto userProfileDto = _mapper.Map<UserProfileDto>(userProfile);
            response = new ApiResponse
            {
                Status = "Success",
                Message = "User Found",
                ResponseValue = userProfileDto
            };
            return Ok(response);
        }

        [Route("search")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> SearchByUserNameAsync(string userName)
        {
            _logger.LogInformation("SearchByUserNameAsync method started...");
            ApiResponse response;
            List<UserProfile> userProfile = await _userRepository.SearchUserAsync(userName);

            if (userProfile == null)
            {
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "User not found!"
                };

                return NotFound(response);
            }

            List<UserDto> userProfileDtos = _mapper.Map<List<UserDto>>(userProfile);
            response = new ApiResponse
            {
                Status = "Success",
                Message = "User Found",
                ResponseValue = userProfileDtos
            };
            return Ok(response);
        }

        [Route("login")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> LoginAsync([FromBody] UserLoginModel user)
        {
            _logger.LogInformation("LoginAsync method started...");

            UserProfile userProfile = await _userRepository.VerifyUserAsync(user.UserName, user.Password);
            if(userProfile == null)
            {
                return BadRequest(new ApiResponse
                {
                    Status = "Error",
                    Message = "Username or Password is incorrect!"
                });
            }
            UserDto userProfileDto = _mapper.Map<UserDto>(userProfile);
            ApiResponse response = new ApiResponse
            {
                Status = "Success",
                Message = "Login Successful",
                ResponseValue = userProfileDto
            };
            return Ok(response);
        }

        [Route("logout")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> LogoutAsync([FromBody] string userName)
        {
            _logger.LogInformation("LogoutAsync method started...");

            ApiResponse response;
            UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(userName);

            userProfile.IsActive = false;
            userProfile.LogoutAt = DateTime.UtcNow;
            bool isLoggedOut = await _userRepository.UpdateUsersAsync(userProfile);

            if(isLoggedOut)
            {
                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Logout Successful",
                };
                return Ok(response);
            }
            else
            {
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Logout Failed. Please try again."
                };
                return BadRequest(response);
            }
        }

        [Route("register")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> RegisterAsync([FromBody] CreateUserProfileDto createUserProfileDto)
        {
            _logger.LogInformation("RegisterAsync method started...");
            ApiResponse response;
            UserProfile addUserProfile = _mapper.Map<UserProfile>(createUserProfileDto);
            bool isUserAdded = await _userRepository.AddUserAsync(addUserProfile);

            if (isUserAdded)
            {
                UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(addUserProfile.LoginId);
                UserDto userProfileDto = _mapper.Map<UserDto>(userProfile);
                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Login Successful",
                    ResponseValue = userProfileDto
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

        [Route("update")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> UpdateProfileAsync([FromBody] UpdateUserProfileDto updateUserProfileDto)
        {
            _logger.LogInformation("UpdateProfileAsync method started...");
            ApiResponse response;
            UserProfile addUserProfile = _mapper.Map<UserProfile>(updateUserProfileDto);
            bool isRecordUpdated = await _userRepository.UpdateUsersAsync(addUserProfile);

            if (isRecordUpdated)
            {
                UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(addUserProfile.LoginId);
                UserProfileDto userProfileDto = _mapper.Map<UserProfileDto>(userProfile);
                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Profile updated successfully.",
                    ResponseValue = userProfileDto
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

        [Route("reset")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> ChangePasswordAsync([FromBody] UserLoginModel user)
        {
            _logger.LogInformation("ChangePasswordAsync method started...");
            ApiResponse response;
            UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(user.UserName);

            userProfile.Password = user.Password;
            bool isUpdated = await _userRepository.UpdateUsersAsync(userProfile);

            if (isUpdated)
            {
                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Password updated successfully",
                };
                return Ok(response);
            }
            else
            {
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Something went wrong. Please try again."
                };
                return BadRequest(response);
            }

        }
       
    }
}
