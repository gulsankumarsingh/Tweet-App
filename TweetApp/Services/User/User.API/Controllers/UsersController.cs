﻿namespace UserService.API.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using User.API.Infrastructure.ActionResults;
    using User.API.Infrastructure.Repository.Interface;
    using User.API.Infrastructure.Services.AuthenticationService.Interfaces;
    using User.API.Infrastructure.Services.AzureServiceBusSender.Interface;
    using User.API.Infrastructure.Services.ImageHandlerService.Interface;
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

        private readonly IDeleteUserMessageSender _deleteUserMessageSender;
        private readonly IHandleImage _handleImage;

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
        /// <param name="deleteUserMessageSender">The deleteUserMessageSender.</param>
        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger, 
                IMapper mapper, IJwtAuthentication jwtAuthentication, IMessageSender messageSender, IDeleteUserMessageSender deleteUserMessageSender, IHandleImage handleImage)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtAuthentication = jwtAuthentication ?? throw new ArgumentNullException(nameof(jwtAuthentication));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _deleteUserMessageSender = deleteUserMessageSender ?? throw new ArgumentNullException(nameof(deleteUserMessageSender));
            _handleImage = handleImage ?? throw new ArgumentNullException(nameof(handleImage));
        }

        /// <summary>
        /// Get All Users
        /// </summary>
        /// <returns>The ApiResponse with status and message/>.</returns>
        [Route("all")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserDto>>> GetAllUsersAsync()
        {

            try
            {
                _logger.LogInformation("GetAllUsers method started...");
                List<UserProfile> userProfiles = await _userRepository.GetAllUsersAsync();
                List<UserDto> userProfileDtos = _mapper.Map<List<UserDto>>(userProfiles);
                return Ok(userProfileDtos);
            }
            catch (Exception ex)
            {

                _logger.LogError("An error occured while getting the All Users", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }

        /// <summary>
        /// Get a user
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
            try
            {
                _logger.LogInformation("GetUserAsync method started...");
                ApiResponse response;
                UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(username.ToLower());

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
            catch(Exception ex)
            {
                _logger.LogError($"An error occured while getting the User detail by username: {username}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }

        /// <summary>
        /// Search a user
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The ApiResponse with status and message/>.</returns>
        [Route("search/{username}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserDto>>> SearchByUserNameAsync([FromRoute] string username)
        {
            try
            {
                _logger.LogInformation("SearchByUserNameAsync method started...");

                List<UserProfile> userProfile = await _userRepository.SearchUserAsync(username);

                if (userProfile != null && userProfile.Count == 0)
                {
                    return Ok(new List<UserDto>());
                }

                List<UserDto> userProfileDtos = _mapper.Map<List<UserDto>>(userProfile);

                return Ok(userProfileDtos);
            }
            catch(Exception ex)
            {
                _logger.LogError($"An error occured while searching the User by username: {username}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }

        /// <summary>
        /// Login
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
            try
            {
                _logger.LogInformation("LoginAsync method started...");

                UserProfile userProfile = await _userRepository.VerifyUserAsync(user.Email.ToLower(), user.Password);
                if (userProfile == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Status = "Error",
                        Message = "Username or Password is incorrect!"
                    });
                }
                userProfile.IsActive = true;
                bool isSuccess = await _userRepository.UpdateUsersAsync(userProfile);
                if (isSuccess)
                {
                    TokenDetail tokenDetail = _jwtAuthentication.GenerateToken(userProfile.Username);
                    ApiResponse response = new ApiResponse
                    {
                        Status = "Success",
                        Message = "Login Successful",
                        ResponseValue = tokenDetail
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

                _logger.LogError($"An error occured while login the by user: {user.Email}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
           
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The ApiResponse with status and message/>.</returns>
        [Route("logout/{username}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> LogoutAsync([FromRoute] string username)
        {
            try
            {
                _logger.LogInformation("LogoutAsync method started...");

                ApiResponse response;
                UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(username.ToLower());

                userProfile.IsActive = false;
                userProfile.LogoutAt = DateTime.Now;
                bool isSuccess = await _userRepository.UpdateUsersAsync(userProfile);
                if (isSuccess)
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
                    return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while logout the user: {username}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
           
        }

        /// <summary>
        /// Register new user
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
            try
            {
                _logger.LogInformation("RegisterAsync method started...");
                ApiResponse response;
                UserProfile user = await _userRepository.GetUserByEmailAsync(createUserProfileDto.Email.ToLower());
                if (user != null)
                {
                    response = new ApiResponse
                    {
                        Status = "EmailExist",
                        Message = "Email already exist. Please login or try different email"
                    };
                    return BadRequest(response);
                }

                user = await _userRepository.GetUserByUserNameAsync(createUserProfileDto.Username.ToLower());
                if (user != null)
                {
                    response = new ApiResponse
                    {
                        Status = "UsernameExist",
                        Message = "Username already taken. Please try different one"
                    };
                    return BadRequest(response);
                }

                UserProfile addUserProfile = _mapper.Map<UserProfile>(createUserProfileDto);
                addUserProfile.Email = addUserProfile.Email.ToLower();
                addUserProfile.Username = addUserProfile.Username.ToLower();
                bool isSuccess = await _userRepository.AddUserAsync(addUserProfile);

                if (isSuccess)
                {
                    response = new ApiResponse
                    {
                        Status = "Success",
                        Message = "Sign up successful",
                        ResponseValue = null
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

                _logger.LogError($"An error occured while registering the user: {createUserProfileDto.Username}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }

        /// <summary>
        /// Update user profile
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
        public async Task<ActionResult<ApiResponse>> UpdateProfileAsync([FromRoute] string username, [FromForm] UpdateUserProfileDto updateUserProfileDto)
        {
            try
            {
                _logger.LogInformation("UpdateProfileAsync method started...");
                ApiResponse response;

                UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(username.ToLower());
                if (userProfile == null)
                {
                    response = new ApiResponse
                    {
                        Status = "Error",
                        Message = "User not found!"
                    };

                    return NotFound(response);
                }
                string uniqueImageName = string.IsNullOrEmpty(userProfile.ImageName) ? Guid.NewGuid().ToString() : userProfile.ImageName;

                if (updateUserProfileDto.ImageFile != null)
                {
                    ImageUploaderResponse uploaderResponse = await _handleImage.UploadImageAsync(updateUserProfileDto.ImageFile, uniqueImageName);
                    if (!uploaderResponse.isUploaded)
                    {
                        return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
                    }
                    else
                    {
                        userProfile.ProfileImage = string.IsNullOrEmpty(uploaderResponse.ImageUrl) ? "" : uploaderResponse.ImageUrl;
                        userProfile.ImageName = uniqueImageName;
                    }
                }

                userProfile.ContactNumber = updateUserProfileDto.ContactNumber;
                userProfile.DateOfBirth = updateUserProfileDto.DateOfBirth;
                userProfile.Email = updateUserProfileDto.Email.ToLower();
                userProfile.FirstName = updateUserProfileDto.FirstName;
                userProfile.LastName = updateUserProfileDto.LastName;
                userProfile.Gender = updateUserProfileDto.Gender;
                userProfile.Status = updateUserProfileDto.Status;

                bool isSuccess = await _userRepository.UpdateUsersAsync(userProfile);

                if (isSuccess)
                {
                    response = new ApiResponse
                    {
                        Status = "Success",
                        Message = "Profile updated successfully."
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

                _logger.LogError($"An error occured while updating the profile for the user: {username}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }

        /// <summary>
        /// Delete user profile
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
            try
            {
                _logger.LogInformation("DeleteProfileAsync method started...");
                ApiResponse response;
                UserProfile profile = await _userRepository.GetUserByUserNameAsync(username.ToLower());
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

                if(!string.IsNullOrEmpty(profile.ImageName) && !string.IsNullOrEmpty(profile.ProfileImage))
                {
                    bool isImageDeleted = await _handleImage.DeleteImageFileAsync(profile.ImageName);
                    if (!isImageDeleted)
                    {
                        return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
                    }
                }
                
                bool isUserDeleted = await _userRepository.DeleteUsersAsync(profile);

                if (!isUserDeleted)
                {
                    return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
                }

                DeleteUserResultMessage deleteUserResultMessage = new DeleteUserResultMessage
                {
                    UserName = username.ToLower()
                };
                //Rabbit Mq sender 
                //Uncomment in case of local run
                //_messageSender.SendMessage(deleteUserResultMessage);

                await _deleteUserMessageSender.PublishMessage(deleteUserResultMessage);

                response = new ApiResponse
                {
                    Status = "Success",
                    Message = "Profile deleted successfully.",
                };
                return Ok(response);
               
            }
            catch (Exception ex)
            {

                _logger.LogError($"An error occured while deleting the profile for the user: {username}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="changePassword">The model with change password essentials.</param>
        /// <returns>The ApiResponse with status and message/>.</returns>
        [AllowAnonymous]
        [Route("{username}/forgetpassword")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> ForgotPasswordAsync([FromRoute] string username, [FromBody] ChangePasswordDto changePassword)
        {
            try
            {
                _logger.LogInformation("ForgotPasswordAsync method started...");
                ApiResponse response;
                UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(username.ToLower());

                if (userProfile == null || userProfile.Email != changePassword.Email.ToLower() || userProfile.ContactNumber != changePassword.ContactNumber)
                {
                    response = new ApiResponse
                    {
                        Status = "Error",
                        Message = "You have entered an invalid detail. Please try again"
                    };
                    return NotFound(response);
                }

                userProfile.Password = changePassword.Password;
                bool isSuccess = await _userRepository.ChangePasswordAsync(userProfile);

                if (isSuccess)
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
                    return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"An error occured while forgot password operation for user: {username}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }

        /// <summary>
        /// Reset Password.
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="resetPasswordDto">resetPasswordDto</param>
        /// <returns>The ApiResponse with status and message/>.</returns>
        [Route("{username}/resetpassword")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> ResetPasswordAsync([FromRoute] string username, [FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                _logger.LogInformation("ResetPasswordAsync method started...");
                ApiResponse response;
                UserProfile userProfile = await _userRepository.GetUserByUserNameAsync(username.ToLower());

                if (userProfile == null)
                {
                    response = new ApiResponse
                    {
                        Status = "Error",
                        Message = "User not found!"
                    };
                    return NotFound(response);
                }
                else if (userProfile.Password != resetPasswordDto.OldPassword)
                {
                    response = new ApiResponse
                    {
                        Status = "Error",
                        Message = "Please enter a valid old password"
                    };
                    return NotFound(response);
                }

                userProfile.Password = resetPasswordDto.NewPassword;
                bool isSuccess = await _userRepository.ChangePasswordAsync(userProfile);

                if (isSuccess)
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
                    return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
                }
         
            }
            catch (Exception ex)
            {

                _logger.LogError($"An error occured while reset password operation for user : {username}", ex.Message);
                return new InternalServerErrorObjectResult(new ApiResponse() { Status = "Error", Message = "Something went wrong! Please try again." });
            }
            
        }
    }
}
