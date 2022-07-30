namespace UserService.API.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using User.API.Infrastructure.Repository.Interface;
    using User.API.Infrastructure.Services.AuthenticationService.Interfaces;
    using User.API.Infrastructure.Services.MessageSenderService.Interface;
    using User.API.Models;
    using User.API.Models.Dtos;

    /// <summary>
    /// Defines the <see cref="UsersController" />.
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// Defines the _userRepository.
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Defines the _jwtAuthentication.
        /// </summary>
        private readonly IJwtAuthentication _jwtAuthentication;

        /// <summary>
        /// Defines the _logger.
        /// </summary>
        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// Defines the _messageSender.
        /// </summary>
        private readonly IMessageSender _messageSender;

        /// <summary>
        /// Defines the _mapper.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="userRepository">The userRepository</param>
        /// <param name="logger">The logger</param>
        /// <param name="mapper">The mapper</param>
        /// <param name="jwtAuthentication">The jwtAuthentication.</param>
        /// <param name="messageSender">The messageSender.</param>
        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger, 
                IMapper mapper, IJwtAuthentication jwtAuthentication, IMessageSender messageSender)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtAuthentication = jwtAuthentication ?? throw new ArgumentNullException(nameof(jwtAuthentication));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        /// <summary>
        /// Method for the get All Users information.
        /// </summary>
        /// <returns>The ApiResponse with status and message/>.</returns>
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

        /// <summary>
        /// Method for Get the User info.
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The ApiResponse with status and message/>.</returns>
        [Route("get/{username}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> GetUserAsync([FromRoute] string username)
        {
            _logger.LogInformation("GetUserAsync method started...");
            ApiResponse response;
            UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(username);

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

        /// <summary>
        /// Method for searching the user by username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The ApiResponse with status and message/>.</returns>
        [Route("search/{username}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> SearchByUserNameAsync([FromRoute] string username)
        {
            _logger.LogInformation("SearchByUserNameAsync method started...");
            ApiResponse response;
            List<UserProfile> userProfile = await _userRepository.SearchUserAsync(username);

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

        /// <summary>
        /// Method for login the user
        /// </summary>
        /// <param name="user">The user login model</param>
        /// <returns>The ApiResponse with status and message/>.</returns>
        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> LoginAsync([FromBody] UserLoginModel user)
        {
            _logger.LogInformation("LoginAsync method started...");

            UserProfile userProfile = await _userRepository.VerifyUserAsync(user.UserName, user.Password);
            if (userProfile == null)
            {
                return BadRequest(new ApiResponse
                {
                    Status = "Error",
                    Message = "Username or Password is incorrect!"
                });
            }
            UserDto userProfileDto = _mapper.Map<UserDto>(userProfile);
            TokenDetail tokenDetail = _jwtAuthentication.GenerateToken(userProfile.LoginId);
            ApiResponse response = new ApiResponse
            {
                Status = "Success",
                Message = "Login Successful",
                ResponseValue = new LoginConfirmationDto
                {
                    TokenDetail = tokenDetail,
                    UserInfo = userProfileDto
                }
            };
            return Ok(response);
        }

        /// <summary>
        /// Method for logout the user
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The ApiResponse with status and message/>.</returns>
        [Route("logout")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> LogoutAsync([FromBody] string username)
        {
            _logger.LogInformation("LogoutAsync method started...");

            ApiResponse response;
            UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(username);

            userProfile.IsActive = false;
            userProfile.LogoutAt = DateTime.UtcNow;
            bool isLoggedOut = await _userRepository.UpdateUsersAsync(userProfile);

            if (isLoggedOut)
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

        /// <summary>
        /// The Register a new user
        /// </summary>
        /// <param name="createUserProfileDto">The model for create a new user data</param>
        /// <returns>The ApiResponse with status and message/>.</returns>
        [AllowAnonymous]
        [Route("register")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
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

        /// <summary>
        /// Method for updating the user information
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="updateUserProfileDto">The model for update user info data</param>
        /// <returns>The ApiResponse with status and message/>.</returns>
        [Route("update/{username}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> UpdateProfileAsync([FromRoute] string username, [FromBody] UpdateUserProfileDto updateUserProfileDto)
        {
            _logger.LogInformation("UpdateProfileAsync method started...");
            ApiResponse response;
            UserProfile profile = await _userRepository.GetUserByUserNameAsync(username);
            if (profile == null)
            {
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "User not found!"
                };

                return NotFound(response);
            }

            profile.Email = updateUserProfileDto.Email;
            profile.FirstName = updateUserProfileDto.FirstName;
            profile.LastName = updateUserProfileDto.LastName;
            profile.Gender = updateUserProfileDto.Gender;
            profile.DateOfBirth = updateUserProfileDto.DateOfBirth;
            profile.ContactNumber = updateUserProfileDto.ContactNumber;
            profile.Status = updateUserProfileDto.Status;
            profile.ProfileImg = updateUserProfileDto.ProfileImg;

            bool isRecordUpdated = await _userRepository.UpdateUsersAsync(profile);

            if (isRecordUpdated)
            {
                UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(profile.LoginId);
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

        /// <summary>
        /// Method for updating the user information
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The ApiResponse with status and message/>.</returns>
        [Route("delete/{username}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> DeleteProfileAsync([FromRoute] string username)
        {
            _logger.LogInformation("DeleteProfileAsync method started...");
            ApiResponse response;
            UserProfile profile = await _userRepository.GetUserByUserNameAsync(username);
            if (profile == null)
            {
                _logger.LogError("User not found", profile);
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "User not found!"
                };

                return NotFound(response);
            }

            bool isRecordDeleted = await _userRepository.DeleteUsersAsync(profile);

            if (isRecordDeleted)
            {
                _logger.LogInformation($"User: {username} profile deleted successfully.");
                
                DeleteUserResultMessage deleteUserResultMessage = new DeleteUserResultMessage
                {
                    UserName = username
                };
                _messageSender.SendMessage(deleteUserResultMessage);
                
                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Profile deleted successfully.",
                };
                return Ok(response);
            }
            else
            {
                _logger.LogError($"User: {username} profile deleted operation failed.");
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "Something went wrong! Please try again"
                };
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Method for reset password using forgot password functionality
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="changePassword">The model with change password essentials.</param>
        /// <returns>The ApiResponse with status and message/>.</returns>
        [Route("{username}/forgetpassword")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> ForgotPasswordAsync([FromRoute] string username, [FromBody] ChangePasswordDto changePassword)
        {
            _logger.LogInformation("ForgotPasswordAsync method started...");
            ApiResponse response;
            UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(username);

            if (userProfile == null || userProfile.ContactNumber != changePassword.ContactNumber)
            {
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "User not found!"
                };
                return NotFound(response);
            }

            userProfile.Password = changePassword.Password;
            bool isUpdated = await _userRepository.ChangePasswordAsync(userProfile);

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

        /// <summary>
        /// The ResetPasswordAsync.
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <returns>The ApiResponse with status and message/>.</returns>
        [Route("{username}/resetpassword")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> ResetPasswordAsync([FromRoute] string username, [FromBody] string password)
        {
            _logger.LogInformation("ResetPasswordAsync method started...");
            ApiResponse response;
            UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(username);

            if (userProfile == null)
            {
                response = new ApiResponse
                {
                    Status = "Error",
                    Message = "User not found!"
                };
                return NotFound(response);
            }

            userProfile.Password = password;
            bool isUpdated = await _userRepository.ChangePasswordAsync(userProfile);

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
